using chd.CaraVan.Devices.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Pi
{
    public class Gpio
    {
        public int Pin { get; set; }
        public PinMode Mode { get; set; }
        public PinValue Default { get; set; }
        public string Name { get; set; }
        public GpioType Type { get; set; }

    }
}
