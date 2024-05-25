using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess.Repositories
{
    public abstract class BaseDataRepository<TData> : IDataRepository<TData> where TData : class, IData
    {
        protected List<TData> _lst;
        protected BaseDataRepository()
        {
            this._lst = new List<TData>();
        }

        public void Add(TData data) => this._lst.Add(data);

        public void Clean(DateTime till)
        {
            this._lst.RemoveAll(x => x.RecordDateTime <= till);
        }

        public void Clear() => this._lst.Clear();

        public IEnumerable<TData> GetAll() => this._lst;

        public IEnumerable<TData> Get(EDataType type, DateTime from, DateTime to) => this._lst.Where(x => x.Type == type && x.RecordDateTime >= from && x.RecordDateTime < to);

    }
    public interface IDataRepository<TData> : IRepository
        where TData : class, IData
    {
        IEnumerable<TData> GetAll();
        IEnumerable<TData> Get(EDataType type, DateTime from, DateTime to);

        void Add(TData data);

        void Clean(DateTime till);
    }

    public interface IRepository
    {
        void Clear();
    }
}
