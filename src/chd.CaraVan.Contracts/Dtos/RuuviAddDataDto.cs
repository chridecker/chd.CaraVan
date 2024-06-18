using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class RuuviAddDataDto
    {
        public int Id { get; set; }
        public RuuviTagDeviceData  Data { get; set; }
    }
}
