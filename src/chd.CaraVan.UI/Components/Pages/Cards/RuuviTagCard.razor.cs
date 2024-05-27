using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class RuuviTagCard : IAsyncDisposable
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private IRuuviTagDataService _dataService { get; set; }
        [Inject] private ITypeNameService _typeNameService { get; set; }

        private CancellationTokenSource _cts = new();
        private RuuviTagDeviceData _data;

        protected override async Task OnInitializedAsync()
        {
            this.ReloadData();
            this.Reload();
            await base.OnInitializedAsync();
        }

        private void Reload() => Task.Run(async () =>
        {
            while (!this._cts.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), this._cts.Token);
                this.ReloadData();
                await this.InvokeAsync(this.StateHasChanged);
            }
        }, this._cts.Token);

        private void ReloadData()
        {
            this._data = this._dataService.GetData(this.DeviceDto.Id, EDataType.Temperature);
        }

        private void NavigateToDevice() => this._navigationManager.NavigateTo($"/ruuvitag/{this.DeviceDto.Id}");


        public async ValueTask DisposeAsync()
        {
            this._cts.Cancel();
        }
    }
}