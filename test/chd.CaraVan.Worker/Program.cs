using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Worker;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders().AddNLog();

builder.Services.Configure<RuuviTagConfiguration>(builder.Configuration.GetSection(nameof(RuuviTagConfiguration)));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
