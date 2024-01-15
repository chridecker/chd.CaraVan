using chd.GeoData.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.GeoData.App.Implementations
{
    public class LocationService : ILocationService
    {

        private Location _lastLocationWithConnection;
        private readonly IGeoLocationHandler _geoLocationHandler;

        public LocationService(IGeoLocationHandler geoLocationHandler)
        {
            this._geoLocationHandler = geoLocationHandler;
        }

        public async Task SetLastLocation(CancellationToken cancellationToken) => this._lastLocationWithConnection = await this._geoLocationHandler.GetCurrentGeoLocation(cancellationToken);

        public async Task<double> GetDistance(CancellationToken cancellationToken)
        {
            if (this._lastLocationWithConnection == null) { return double.NegativeZero; }
            return Location.CalculateDistance(await this._geoLocationHandler.GetCurrentGeoLocation(cancellationToken), this._lastLocationWithConnection, DistanceUnits.Kilometers);
        }


    }
}
