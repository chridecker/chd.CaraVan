using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class DeviceData : DataBase<int>
    {
        public int DeviceId { get; set; }
        public DeviceData(int id, DateTime time, EDataType type, decimal val) : base(id, time, type, val)
        {
        }
    }
}
