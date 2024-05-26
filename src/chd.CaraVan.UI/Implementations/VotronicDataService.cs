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
        public VotronicBatteryData GetBatteryData() => this._votronicBatteryData;
        public VotronicSolarData GetSolarData() => this._votronicSolarData;
    }
    public interface IVotronicDataService
    {
        void AddData(VotronicBatteryData votronicBatteryData);
        void AddData(VotronicSolarData votronicSolarData);
        VotronicBatteryData GetBatteryData();
        VotronicSolarData GetSolarData();
    }
}
