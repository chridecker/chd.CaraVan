using chd.GeoData.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.GeoData.App.Implementations.Android
{
    public class ConnectivityHandler : IConnectivityHandler
    {

        public event EventHandler<NetworkAccess> NetworkStateChanges;

        public ConnectivityHandler()
        {
            Connectivity.ConnectivityChanged += this.Connectivity_ConnectivityChanged;
        }


        public void CheckConnectivity() => this.NetworkStateChanges?.Invoke(this, Connectivity.NetworkAccess);

        private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            this.NetworkStateChanges?.Invoke(this, e.NetworkAccess);
        }
        ~ConnectivityHandler()
        {
            Connectivity.ConnectivityChanged -= this.Connectivity_ConnectivityChanged;
        }
    }
}
