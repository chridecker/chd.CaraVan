using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.RuvviTag
{
    public class RuuviTagConfiguration
    {
        public string BLEAdapter { get; set; }
        public string DeviceAddress { get; set; }
    }
}
