using chd.CaraVan.Devices.Contracts.Dtos.Pi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices
{
    public class PiManager : IPiManager
    {
        private readonly ILogger<PiManager> _logger;
        private readonly IOptionsMonitor<PiSettings> _optionsMonitor;

        private GpioController _controller;

        public PiManager(ILogger<PiManager> logger, IOptionsMonitor<PiSettings> optionsMonitor)
        {
            this._logger = logger;
            this._optionsMonitor = optionsMonitor;
        }

        public Task<PiSettings> GetSettings(CancellationToken cancellationToken) => Task.FromResult(this._optionsMonitor.CurrentValue);
        public void Start()
        {
            this._controller = new GpioController();
            foreach (var g in this._optionsMonitor.CurrentValue.Gpios)
            {
                if (this._controller.IsPinOpen(g.Pin))
                {
                    this._controller.ClosePin(g.Pin);
                }
                this._controller.OpenPin(g.Pin, g.Mode, g.Default);
            }
        }

        public void Stop()
        {
            foreach (var g in this._optionsMonitor.CurrentValue.Gpios)
            {
                if (this._controller.IsPinOpen(g.Pin))
                {
                    this._controller.ClosePin(g.Pin);
                }
            }
            this._controller?.Dispose();
        }

        public Task Write(int pin, bool val) => Task.Run(() => this.WriteToPin(pin, val));
        public Task<bool> Read(int pin) => Task.FromResult(this._controller?.Read(pin) == PinValue.High);

        public async Task StartPiTunnel(CancellationToken cancellationToken = default)
            => _ = await this.RunProcess("sudo service ", "pitunnel start", cancellationToken);

        public async Task StopPiTunnel(CancellationToken cancellationToken = default)
            => _ = await this.RunProcess("sudo service ", "pitunnel stop", cancellationToken);

        public async Task<bool> IsPiTunnelRunning(CancellationToken cancellationToken = default)
        {
            var output = await this.RunProcess("sudo ps", "-x | grep \"pitunnel\"", cancellationToken);
            return !string.IsNullOrWhiteSpace(output);
        }

        private void WriteToPin(int pin, bool val) => this._controller?.Write(pin, val ? PinValue.High : PinValue.Low);

        private async Task<string> RunProcess(string filename, string args, CancellationToken cancellationToken)
        {
            var info = new ProcessStartInfo(filename, args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = true
            };
            var proc = Process.Start(info);
            proc.Start();
            await proc.WaitForExitAsync(cancellationToken);
            return proc.StandardOutput.ReadToEnd();
        }
    }
    public interface IPiManager
    {
        Task<PiSettings> GetSettings(CancellationToken cancellationToken = default);
        Task<bool> IsPiTunnelRunning(CancellationToken cancellationToken = default);
        Task<bool> Read(int pin);
        void Start();
        Task StartPiTunnel(CancellationToken cancellationToken = default);
        void Stop();
        Task StopPiTunnel(CancellationToken cancellationToken = default);
        Task Write(int pin, bool val);
    }
}
