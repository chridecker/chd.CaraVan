using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos.Base
{
    public abstract class DataBase<T> : IData<T> where T : struct
    {
        public T Id { get; }
        public DateTime RecordDateTime { get; }
        public EDataType Type { get; }
        public decimal Value { get; }

        protected DataBase(T id, DateTime time, EDataType type, decimal val)
        {
            this.Id = id;
            this.RecordDateTime = time;
            this.Type = type;
            this.Value = val;
        }
    }
}
