using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Votronic
{
    public class VotronicEventArgs
    {
        public DateTime DateTime { get; set; }
        public VotronicBatteryData? BatteryData { get; set; }
        public VotronicSolarData? SolarData { get; set; }
    }
}
