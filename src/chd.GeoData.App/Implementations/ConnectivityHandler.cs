
using chd.GeoData.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.GeoData.App.Implementations
{
    public class ConnectivityHandler : IConnectivityHandler
    {

        public event EventHandler<NetworkAccess> NetworkStateChanges;

        public ConnectivityHandler()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }


        public void CheckConnectivity() => NetworkStateChanges?.Invoke(this, Connectivity.NetworkAccess);

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            NetworkStateChanges?.Invoke(this, e.NetworkAccess);
        }
        ~ConnectivityHandler()
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
    }
}
