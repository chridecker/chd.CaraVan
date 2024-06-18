using chd.CaraVan.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class VictronDataService : IVictronDataService
    {
        private VictronData _data;
        public Task Add(VictronData data, CancellationToken cancellationToken = default) => Task.Run(()=>{this._data = data;}, cancellationToken);
        public Task<VictronData> GetData(CancellationToken cancellationToken) => Task.FromResult(this._data);
    }
    public interface IVictronDataService
    {
        Task Add(VictronData data, CancellationToken cancellationToken = default);
        Task<VictronData> GetData(CancellationToken cancellationToken = default);
    }
}
