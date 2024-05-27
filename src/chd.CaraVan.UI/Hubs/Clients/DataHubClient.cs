using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.UI.Hubs.Clients.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Hubs.Clients
{
    public class DataHubClient : BaseHubClient, IDataHubClient
    {
        public event EventHandler<VotronicData> VotronicDataReceived;
        public event EventHandler<RuuviTagDeviceData> RuuviTagDeviceDataReceived;

        public DataHubClient(ILogger<DataHubClient> logger) : base(logger)
        {
        }


        protected override Task DoInvokations(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override void HookIncomingCalls()
        {
            this._connection.On<VotronicData>(nameof(IDataHub.VotronicData), (data) =>
            {
                this.VotronicDataReceived?.Invoke(this, data);
            });

            this._connection.On<RuuviTagDeviceData>(nameof(IDataHub.RuuviTagData), (data) =>
            {
                this._logger?.LogDebug($"Received Ruuvi");
                this.RuuviTagDeviceDataReceived?.Invoke(this, data);
            });
        }

        protected override Task<bool> ShouldInitialize(CancellationToken cancellationToken) => Task.FromResult(true);

        protected override void SpecificReinitialize()
        {
            this._connection.Remove(nameof(IDataHub.VotronicData));
            this._connection.Remove(nameof(IDataHub.RuuviTagData));
        }
    }
    public interface IDataHubClient : IBaseHubClient
    {
        event EventHandler<VotronicData> VotronicDataReceived;
        event EventHandler<RuuviTagDeviceData> RuuviTagDeviceDataReceived;
    }
}
