using chd.CaraVan.Devices;
using chd.CaraVan.Devices.Contracts.Dtos.RuvviTag;
using chd.CaraVan.Devices.Contracts.Dtos.Votronic;

namespace chd.CaraVan.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private BLEManager _tag;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task StartDevices(CancellationToken cancellationToken)
        {
            this._tag = new BLEManager(this._logger, Enumerable.Empty<RuuviTagConfiguration>(),
                new VotronicConfiguration
                {
                    Alias = "Votronic",
                    DeviceAddress = "",
                    Id = 4,
                    BatteryAH = 200,
                });
            //.Select(s => new RuuviTagConfiguration
            //{
            //    Alias = s.Name,
            //    DeviceAddress = s.UID,
            //    Id = s.Id,
            //}), new VotronicConfiguration
            //{
            //    Id = this._optionsMonitor.CurrentValue.Votronic?.Id ?? 0,
            //    DeviceAddress = this._optionsMonitor.CurrentValue.Votronic?.UID,
            //    Alias = this._optionsMonitor.CurrentValue.Votronic?.Name
            //});

            this._tag.VotronicDataReceived += this._tag_VotronicDataReceived; ;
            await this._tag.ConnectAsync(cancellationToken);
        }

        private void _tag_VotronicDataReceived(object? sender, VotronicEventArgs e)
        {

        }
    }
}
