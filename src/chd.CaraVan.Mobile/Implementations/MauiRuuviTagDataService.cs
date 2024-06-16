using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiRuuviTagDataService : BaseMauiService, IRuuviTagDataService
    {
        public MauiRuuviTagDataService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public Task<IEnumerable<RuuviDeviceDto>> Devices => this.Get<IEnumerable<RuuviDeviceDto>>("RuuviSensorDevices");

        public void AddData(int id, RuuviTagDeviceData data)
        {
            throw new NotImplementedException();
        }

        public Task<RuuviSensorDataDto> GetData(int id, CancellationToken cancellationToken = default) => this.Get<RuuviSensorDataDto>($"RuuviSensorData/{id}", cancellationToken);

        public Task<(decimal? Min, decimal? Max)> GetMinMaxData(int id, EDataType type, CancellationToken cancellationToken = default)
        => this.Get<(decimal?, decimal?)>($"RuuviSensorDataMinMax/{id}", cancellationToken);
    }
}
