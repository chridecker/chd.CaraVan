using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Hubs.Clients;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class RuuviTagCard
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private IRuuviTagDataService _dataService { get; set; }
        [Inject] private IDataHubClient _dataHubClient { get; set; }

        private RuuviTagDeviceData _data;

        protected override async Task OnInitializedAsync()
        {
            this._dataHubClient.RuuviTagDeviceDataReceived += this._dataHubClient_RuuviTagDeviceDataReceived;
            _data = await this._dataService.GetData(this.DeviceDto.Id);
            await base.OnInitializedAsync();
        }

        private decimal? _min => 0;
        private decimal? _max => 0;

        private async void _dataHubClient_RuuviTagDeviceDataReceived(object? sender, EventArgs e)
        {
            this._data = await this._dataService.GetData(this.DeviceDto.Id);
            await this.InvokeAsync(this.StateHasChanged);
        }

        private void NavigateToDevice() => this._navigationManager.NavigateTo($"/ruuvitag/{this.DeviceDto.Id}");
    }
}