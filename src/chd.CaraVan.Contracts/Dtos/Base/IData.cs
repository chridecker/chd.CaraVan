using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos.Base
{
    public interface IData
    {
        DateTime RecordDateTime { get; }
        EDataType Type { get; }
        decimal Value { get; }
    }
}
