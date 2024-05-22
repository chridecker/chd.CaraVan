using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class DeviceCard : IDisposable
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private IDataService _dataService { get; set; }

        private CancellationTokenSource _cts = new();
        private (decimal?, DateTime?) _temperature;
        private (decimal?, DateTime?) _humidity;
        private Task _reload;

        private string _data(decimal? data) => !data.HasValue ? "NaN" : data.Value.ToString("n2");

        protected override async Task OnInitializedAsync()
        {
            this._reload = this.ReloadData(this._cts.Token);

            await base.OnInitializedAsync();
        }
        private Task ReloadData(CancellationToken cancellationToken) => Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var temp = await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, EDataType.Temperature, cancellationToken);
                var humidity = await this._dataService.GetLatestDataToDeviceAsync(this.DeviceDto.Id, EDataType.Humidity, cancellationToken);
                this._temperature = (temp?.Value, temp?.RecordDateTime);
                this._humidity = (humidity?.Value, humidity?.RecordDateTime);
                await this.InvokeAsync(this.StateHasChanged);
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

            }
        }, cancellationToken);

        private void NavigateToDevice() => this._navigationManager.NavigateTo($"/device/{this.DeviceDto.Id}");

        public void Dispose()
        {
            this._cts.Cancel();
        }
    }
}