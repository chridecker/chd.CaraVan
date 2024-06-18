using chd.CaraVan.Contracts.Settings;
using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.Pi;
using chd.CaraVan.UI.Hubs.Clients;
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
            services.Configure<AesSettings>(configuration.GetSection(nameof(AesSettings)));
            services.Configure<PiSettings>(configuration.GetSection(nameof(PiSettings)));

            services.AddScoped<IDataHubClient, DataHubClient>();
            services.AddSingleton<ISettingService, SettingService>();
            services.AddSingleton<IAESManager, AESManager>();
            services.AddSingleton<IPiManager, PiManager>();
            services.AddSingleton<IVictronDataService, VictronDataService>();
            services.AddSingleton<IRuuviTagDataService, RuuviTagDataService>();
            services.AddSingleton<IVotronicDataService, VotronicDataService>();

            services.AddMudServices();

            if (OperatingSystem.IsLinux())
            {
                services.AddHostedService<DeviceWorker>();
            }

            return services;
        }
    }
}
