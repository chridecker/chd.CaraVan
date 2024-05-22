using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class DeviceCard
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private IDataService _dataService { get; set; }


        private decimal? _temperature;
        private decimal? _humidity;

        private string _data(decimal? data) => !data.HasValue ? "NaN" : data.Value.ToString("n2");

        protected override async Task OnInitializedAsync()
        {
            this._temperature = (await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, Contracts.Enums.EDataType.Temperature))?.Value;
            this._humidity = (await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, Contracts.Enums.EDataType.Humidity))?.Value;
            await base.OnInitializedAsync();
        }

        private void NavigateToDevice()
        {
            this._navigationManager.NavigateTo($"/device/{this.DeviceDto.Id}");
        }
    }
}