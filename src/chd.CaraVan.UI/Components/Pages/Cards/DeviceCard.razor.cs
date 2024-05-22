using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class DeviceCard
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private IDataService _dataService { get; set; }


        private (decimal?, DateTime?) _temperature;
        private (decimal?, DateTime?) _humidity;

        private string _data(decimal? data) => !data.HasValue ? "NaN" : data.Value.ToString("n2");

        protected override async Task OnInitializedAsync()
        {
            var temp = await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, EDataType.Temperature);
            var humidity = await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, EDataType.Humidity);
            this._temperature = (temp?.Value, temp?.RecordDateTime);
            this._humidity = (humidity?.Value, humidity?.RecordDateTime);
            await base.OnInitializedAsync();
        }

        private void NavigateToDevice()
        {
            this._navigationManager.NavigateTo($"/device/{this.DeviceDto.Id}");
        }
    }
}