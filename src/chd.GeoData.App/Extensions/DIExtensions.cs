using chd.GeoData.App.Implementations;
using chd.GeoData.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.GeoData.App.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddChdLogic(this IServiceCollection services)
        {
            services.AddSingleton<IConnectivityHandler, ConnectivityHandler>();
            services.AddSingleton<ILocationService, LocationService>();

            services.AddSingleton<IGeoLocationHandler, GeoLocationHandler>();

            return services;
        }
    }
}
