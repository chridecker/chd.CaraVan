using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.RuvviTag
{
    public class RuuviTagEventArgs
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public RuuviTagData Data { get; set; }
    }
}
