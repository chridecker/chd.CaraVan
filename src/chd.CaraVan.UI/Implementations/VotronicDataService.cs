using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class VotronicDataService : IVotronicDataService
    {
        private readonly IInfluxContext _influxContext;


        private VotronicBatteryData _votronicBatteryData;
        private VotronicSolarData _votronicSolarData;

        public VotronicDataService(IInfluxContext influxContext)
        {
            this._influxContext = influxContext;
        }
        public async Task AddData(VotronicSolarData data)
        {
            await this._influxContext.WriteSolarData(data.DateTime, data.Ampere, data.State);
            this._votronicSolarData = data;
        }
        public async Task AddData(VotronicBatteryData data)
        {
            await this._influxContext.WriteBatteryData(data.DateTime, data.Ampere, data.AmpereH, data.Percent, data.Voltage);
            this._votronicBatteryData = data;
        }
        public Task<VotronicBatteryData> GetBatteryData() => Task.FromResult(this._votronicBatteryData);
        public Task<VotronicSolarData> GetSolarData() => Task.FromResult(this._votronicSolarData);
    }
    public interface IVotronicDataService
    {
        Task AddData(VotronicBatteryData votronicBatteryData);
        Task AddData(VotronicSolarData votronicSolarData);
        Task<VotronicBatteryData> GetBatteryData();
        Task<VotronicSolarData> GetSolarData();
    }
}
