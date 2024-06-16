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
        public void Add(VictronData data) => this._data = data;
        public Task<VictronData> GetData(CancellationToken cancellationToken) => Task.FromResult(this._data);
    }
    public interface IVictronDataService
    {
        void Add(VictronData data);
        Task<VictronData> GetData(CancellationToken cancellationToken = default);
    }
}
