using chd.CaraVan.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUi(this IServiceCollection services)
        {
            services.AddTransient<IDataService, DataService>();

            return services;
        }
    }
}
