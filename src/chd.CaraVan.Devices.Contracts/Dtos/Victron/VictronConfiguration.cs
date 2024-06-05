using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Victron
{
    public class VictronConfiguration
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string DeviceAddress { get; set; }
    }
}
