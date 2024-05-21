using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UID { get; set; }
        public EDataType[] Types{ get; set; }
    }
}
