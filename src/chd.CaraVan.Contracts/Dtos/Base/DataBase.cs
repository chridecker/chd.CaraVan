using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos.Base
{
    public abstract class DataBase : IData 
    {
        public DateTime RecordDateTime { get; }
        public EDataType Type { get; }
        public decimal Value { get; }

        protected DataBase(DateTime time, EDataType type, decimal val)
        {
            this.RecordDateTime = time;
            this.Type = type;
            this.Value = val;
        }
    }
}
