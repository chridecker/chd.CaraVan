using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Victron
{
    public class VictronEventArgs
    {
        public DateTime DateTime { get; set; }
        public VictronData Data { get; set; }
    }
}
