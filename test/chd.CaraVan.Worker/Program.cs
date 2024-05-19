using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Worker;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders().AddNLog();

builder.Services.Configure<RuvviTagConfiguration>(builder.Configuration.GetSection(nameof(RuvviTagConfiguration)));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
