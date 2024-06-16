using chd.CaraVan.Devices;
using chd.CaraVan.Mobile.Implementations;
using chd.CaraVan.UI.Hubs.Clients;
using chd.CaraVan.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddMauiUI(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpClient(sp =>
            {
                return new Uri($"{sp.GetRequiredService<ISettingService>().GetDataHubUri()}api/");
            });

            services.AddScoped<IDataHubClient, DataHubClient>();

            services.AddSingleton<ISettingService, MauiSettingService>();


            services.AddSingleton<IVictronDataService, MauiVictronDataService>();
            services.AddSingleton<IRuuviTagDataService, MauiRuuviTagDataService>();
            services.AddSingleton<IVotronicDataService, MauiVotronicDataService>();
            services.AddSingleton<IAESManager, MauiAESManager>();
            services.AddSingleton<IPiManager, MauiPiManager>();

            services.AddMudServices();

            return services;

        }
        public static HttpClient CreateDefaultClient(this IHttpClientFactory httpClientFactory) => httpClientFactory.CreateClient("Default");
        private static IServiceCollection AddHttpClient(this IServiceCollection services, Func<IServiceProvider, Uri> func)
        {
            services.AddHttpClient("Default", configureClient =>
            {
                configureClient.BaseAddress = func(services.BuildServiceProvider());
            });
            return services;
        }
    }
}
