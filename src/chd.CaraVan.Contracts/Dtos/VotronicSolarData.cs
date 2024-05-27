using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class VotronicSolarData : VotronicData
    {
        public string State { get; set; }
        public decimal WattH { get; set; }
    }
}
