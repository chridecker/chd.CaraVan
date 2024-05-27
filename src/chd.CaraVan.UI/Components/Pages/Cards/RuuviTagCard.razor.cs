using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.Contracts.Enums;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;

namespace chd.CaraVan.UI.Components.Pages.Cards
{
    public partial class RuuviTagCard
    {
        [Parameter] public DeviceDto DeviceDto { get; set; }

        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private IRuuviTagDataService _dataService { get; set; }

        private RuuviTagDeviceData _data;

        protected override async Task OnInitializedAsync()
        {
            this.ReloadData();
            await base.OnInitializedAsync();
        }

        private void ReloadData()
        {
            this._data = this._dataService.GetData(this.DeviceDto.Id, EDataType.Temperature);
        }

        private void NavigateToDevice() => this._navigationManager.NavigateTo($"/ruuvitag/{this.DeviceDto.Id}");
    }
}