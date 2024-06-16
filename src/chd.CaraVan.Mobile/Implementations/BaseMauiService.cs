using chd.CaraVan.Mobile.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public abstract class BaseMauiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        protected HttpClient _httpClient => this._httpClientFactory.CreateDefaultClient();

        protected BaseMauiService(IHttpClientFactory httpClientFactory)
                {
            this._httpClientFactory = httpClientFactory;
        }

        protected Task<T> Get<T>(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                return this._httpClient.GetFromJsonAsync<T>(path, cancellationToken);
            }
            catch (Exception ex) { }
            return Task.FromResult(default(T));

        }
    }
}
