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
        public event EventHandler VotronicDataReceived;
        public event EventHandler VictronDataReceived;
        public event EventHandler RuuviTagDeviceDataReceived;

        public DataHubClient(ILogger<DataHubClient> logger) : base(logger)
        {
        }


        protected override Task DoInvokations(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override void HookIncomingCalls()
        {
            this._connection.On(nameof(IDataHub.VotronicData), () =>
            {
                this.VotronicDataReceived?.Invoke(this, EventArgs.Empty);
            });

            this._connection.On(nameof(IDataHub.RuuviTagData), () =>
            {
                this.RuuviTagDeviceDataReceived?.Invoke(this, EventArgs.Empty);
            });
            this._connection.On(nameof(IDataHub.VictronData), () =>
            {
                this.VictronDataReceived?.Invoke(this, EventArgs.Empty);
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
        event EventHandler VotronicDataReceived;
        event EventHandler VictronDataReceived;
        event EventHandler RuuviTagDeviceDataReceived;
    }
}
