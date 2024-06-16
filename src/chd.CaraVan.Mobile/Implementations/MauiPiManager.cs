using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiPiManager : BaseMauiService, IPiManager
    {
        public MauiPiManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public Task<bool> Read(int pin) => this.Get<bool>($"pi/Read/{pin}");

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public Task Write(int pin, bool val) => this._httpClient.PostAsync("pi/Write", JsonContent.Create(new PinWriteDto
        {
            Pin = pin,
            Value = val,
        }));
    }
}
