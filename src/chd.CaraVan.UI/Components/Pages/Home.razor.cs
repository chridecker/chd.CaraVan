// <auto-generated/>
using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.UI.Hubs.Clients;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.UI.Components.Pages
{
    public partial class Home : IDisposable
    {
        [Inject] private IOptionsMonitor<DeviceSettings> DeviceSettings { get; set; }
        [Inject] private IVotronicDataService _votronicData { get; set; }
        [Inject] private IDataHubClient _dataHubClient { get; set; }
        [Inject] private NavigationManager? _navigationManager { get; set; }

        private VotronicBatteryData VotronicBatteryData => this._votronicData.GetBatteryData();
        private VotronicSolarData VotronicSolarData => this._votronicData.GetSolarData();

        private void NavigateToBattery() => this._navigationManager.NavigateTo($"/battery");
        private void NavigateToSolar() => this._navigationManager.NavigateTo($"/solar");

        protected override async Task OnInitializedAsync()
        {
            if (!this._dataHubClient.IsConnected)
            {
                this.StartHub();
            }
            this._dataHubClient.VotronicDataReceived += this._dataHubClient_VotronicDataReceived;
            this._dataHubClient.RuuviTagDeviceDataReceived += this._dataHubClient_RuuviTagDeviceDataReceived;
            await base.OnInitializedAsync();
        }

        private async void _dataHubClient_RuuviTagDeviceDataReceived(object sender, EventArgs e) => await this.InvokeAsync(this.StateHasChanged);

        private async void _dataHubClient_VotronicDataReceived(object sender, EventArgs e) => await this.InvokeAsync(this.StateHasChanged);

        private void StartHub() => Task.Run(async () => await this._dataHubClient.StartAsync(this._navigationManager.BaseUri));
        public void Dispose()
        {
            this._dataHubClient.VotronicDataReceived -= this._dataHubClient_VotronicDataReceived;
        }
    }
}