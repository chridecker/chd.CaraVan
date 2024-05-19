using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using Microsoft.Extensions.Options;

namespace chd.CaraVan.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<RuvviTagConfiguration> _optionsMonitor;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<RuvviTagConfiguration> optionsMonitor)
        {
            _logger = logger;
            this._optionsMonitor = optionsMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            var tag = new RuuviTag(this._logger, new RuvviTagConfiguration
            {
                BLEAdapter = this._optionsMonitor.CurrentValue.BLEAdapter,
                DeviceAddress = this._optionsMonitor.CurrentValue.DeviceAddress
            });
            await tag.ConnectAsync(stoppingToken);


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
