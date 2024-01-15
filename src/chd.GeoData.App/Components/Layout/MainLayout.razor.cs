using chd.GeoData.App.Interfaces;
using Microsoft.AspNetCore.Components;

namespace chd.GeoData.App.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] private IConnectivityHandler _connectivityHandler { get; set; }
        [Inject] private ILocationService _locationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this._connectivityHandler.NetworkStateChanges += this._connectivityHandler_NetworkStateChanges;
            await base.OnInitializedAsync();
        }

        private async void _connectivityHandler_NetworkStateChanges(object? sender, NetworkAccess e)
        {
            if (e != NetworkAccess.Internet)
            {
                await this._locationService.SetLastLocation();
            }
        }
    }
}