using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class VictronData
    {
        public decimal AmpereAC { get; set; }
        public decimal AmpereDC { get; set; }
        public byte State { get; set; }
        public byte Error { get; set; }
        public DateTime DateTime { get; set; }
    }
}
