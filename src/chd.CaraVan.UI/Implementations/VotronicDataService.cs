using chd.CaraVan.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class VotronicDataService : IVotronicDataService
    {
        private VotronicBatteryData _votronicBatteryData;

        private VotronicSolarData _votronicSolarData;

        public void AddData(VotronicSolarData votronicSolarData) => this._votronicSolarData = votronicSolarData;
        public void AddData(VotronicBatteryData votronicBatteryData) => this._votronicBatteryData = votronicBatteryData;
        public Task<VotronicBatteryData> GetBatteryData(CancellationToken cancellationToken = default) => Task.FromResult(this._votronicBatteryData);
        public Task<VotronicSolarData> GetSolarData(CancellationToken cancellationToken = default) => Task.FromResult(this._votronicSolarData);
    }
    public interface IVotronicDataService
    {
        void AddData(VotronicBatteryData votronicBatteryData);
        void AddData(VotronicSolarData votronicSolarData);
        Task<VotronicBatteryData> GetBatteryData(CancellationToken cancellationToken = default);
        Task<VotronicSolarData> GetSolarData(CancellationToken cancellationToken = default);
    }
}
