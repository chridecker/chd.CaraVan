using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.Pi
{
    public class PiSettings
    {
        public IEnumerable<Gpio> Gpios { get; set; }
    }
}
