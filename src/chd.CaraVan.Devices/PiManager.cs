using chd.CaraVan.Devices.Contracts.Dtos.Pi;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices
{
    public class PiManager : IPiManager
    {
        private readonly IOptionsMonitor<PiSettings> _optionsMonitor;

        private GpioController _controller;
        private IDictionary<int, GpioPin> _pinDict = new Dictionary<int, GpioPin>();

        public PiManager(IOptionsMonitor<PiSettings> optionsMonitor)
        {
            this._optionsMonitor = optionsMonitor;
        }
        public void Start()
        {
            this._controller = new GpioController();
            foreach (var g in this._optionsMonitor.CurrentValue.Gpios)
            {
                var p = this._controller.OpenPin(g.Pin, g.Mode);
                this._pinDict[g.Pin] = p;
                this.WriteToPin(p.PinNumber, g.Default);
            }
        }

        public void Stop()
        {
            this._controller?.Dispose();
        }

        public void Write(int pin, bool val) => this.WriteToPin(pin, val);
        public bool Read(int pin) => this._controller.Read(pin) == PinValue.High;
        private void WriteToPin(int pin, bool val)
        {
            if (this._pinDict.TryGetValue(pin, out var p))
            {
                var currentVal = p.Read() == PinValue.High;
                if (val != currentVal)
                {
                    this._controller.Write(p.PinNumber, val ? PinValue.High : PinValue.Low);
                }
            }
        }

    }
    public interface IPiManager
    {
        bool Read(int pin);
        void Start();
        void Stop();
        void Write(int pin, bool val);
    }
}
