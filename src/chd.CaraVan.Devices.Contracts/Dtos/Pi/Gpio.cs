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
        public PinMode  Mode{ get; set; }
        public bool Default { get; set; }
    }
}
