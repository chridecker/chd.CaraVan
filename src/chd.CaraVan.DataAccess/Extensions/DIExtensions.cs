using chd.CaraVan.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services) => services.AddTransient<IDeviceDataRepository, DeviceDataRepository>();
    }
}
