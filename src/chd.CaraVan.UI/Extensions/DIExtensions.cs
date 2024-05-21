﻿using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.UI.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUi(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<DeviceSettings>(configuration.GetSection(nameof(DeviceSettings)));

            services.AddTransient<IDataService, DataService>();

            services.AddMudServices();


            return services;
        }
    }
}