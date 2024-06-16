using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.UI.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiVotronicDataService : BaseMauiService, IVotronicDataService
    {
        public MauiVotronicDataService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public void AddData(VotronicBatteryData votronicBatteryData)
        {
            throw new NotImplementedException();
        }

        public void AddData(VotronicSolarData votronicSolarData)
        {
            throw new NotImplementedException();
        }

        public Task<VotronicBatteryData> GetBatteryData(CancellationToken cancellationToken)
        => this.Get<VotronicBatteryData>("VotronicBattery", cancellationToken);

        public Task<VotronicSolarData> GetSolarData(CancellationToken cancellationToken)
        => this.Get<VotronicSolarData>("VotronicSolar", cancellationToken);
    }
}
