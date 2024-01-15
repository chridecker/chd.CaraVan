using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.GeoData.App.Interfaces
{
    public interface IGeoLocationHandler
    {
        Task<Location> GetCurrentGeoLocation(CancellationToken cancellationToken);
    }
}
