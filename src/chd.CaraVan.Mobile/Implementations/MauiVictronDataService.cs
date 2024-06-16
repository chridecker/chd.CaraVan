using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Mobile.Extensions;
using chd.CaraVan.UI.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiVictronDataService : BaseMauiService, IVictronDataService
    {
        public MauiVictronDataService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }
        public void Add(VictronData data)
        {
            throw new NotImplementedException();
        }

        public Task<VictronData> GetData(CancellationToken cancellationToken = default)
            => this._httpClient.GetFromJsonAsync<VictronData>("Victron", cancellationToken);

    }
}
