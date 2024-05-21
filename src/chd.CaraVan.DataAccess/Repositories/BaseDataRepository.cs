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
    public abstract class BaseDataRepository<TData, TId> : IDataRepository<TData, TId> where TData : class, IData<TId> where TId : struct
    {
        protected List<TData> _bag;
        protected BaseDataRepository()
        {
            this._bag = new List<TData>();
        }

        public void Add(TData data) => this._bag.Add(data);

        public void Clean(EDataType type, DateTime till)
        {
            this._bag.RemoveAll(x => x.Type == type && x.RecordDateTime <= till);
        }

        public void Clear() => this._bag.Clear();

        public Task<IEnumerable<TData>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult((IEnumerable<TData>)this._bag.ToList());

        public async Task<IEnumerable<TData>> GetAsync(EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        => this._bag.Where(x => x.Type == type && x.RecordDateTime >= from && x.RecordDateTime < to);

    }
    public interface IDataRepository<TData, TId> : IRepository
        where TData : class, IData<TId> where TId : struct
    {
        Task<IEnumerable<TData>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TData>> GetAsync(EDataType type, DateTime from, DateTime to, CancellationToken cancellationToken = default);

        void Add(TData data);

        void Clean(EDataType type, DateTime till);
    }

    public interface IRepository
    {
        void Clear();
    }
}
