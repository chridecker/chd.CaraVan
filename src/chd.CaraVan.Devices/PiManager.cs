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
        private void WriteToPin(int pin, bool val) => this._controller?.Write(pin, val ? PinValue.High : PinValue.Low);
    }
    public interface IPiManager
    {
        Task<PiSettings> GetSettings(CancellationToken cancellationToken = default);
        Task<bool> Read(int pin);
        void Start();
        void Stop();
        Task Write(int pin, bool val);
    }
}
