using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class VotronicBatteryData
    {
        public decimal Voltage { get; set; }
        public decimal AmpereH { get; set; }
        public decimal Percent { get; set; }
        public decimal Ampere { get; set; }
    }
}
