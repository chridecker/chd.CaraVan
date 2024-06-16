using chd.CaraVan.UI.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiAESManager : BaseMauiService, IAESManager
    {
        public MauiAESManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public Task<bool> IsActive => this.Get<bool>("aes/IsActive");

        public Task<DateTime?> SolarAesOffSince => this.Get<DateTime?>("aes/SolarAesOffSince");
        public Task<decimal?> BatteryLimit => this.Get<decimal?>("aes/BatteryLimit");
        public Task<decimal?> SolarAmpLimit => this.Get<decimal?>("aes/SolarAmpLimit");
        public Task<TimeSpan?> AesTimeout =>    this.Get<TimeSpan?>("aes/aesTimeout");


        public event EventHandler<bool> StateSwitched;

        public Task CheckForActive() => this._httpClient.PostAsync("aes/CheckForActive", new StringContent(""));


        public Task Off() => this._httpClient.PostAsync("aes/Off", new StringContent(""));

        public Task SetActive() => this._httpClient.PostAsync("aes/SetActive", new StringContent(""));
    }
}
