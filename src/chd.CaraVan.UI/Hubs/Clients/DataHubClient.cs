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
        public event EventHandler<(int, RuuviTagDeviceData)> RuuviTagDeviceDataReceived;

        public DataHubClient(ILogger<DataHubClient> logger) : base(logger)
        {
        }


        protected override async Task DoInvokations(CancellationToken cancellationToken)
        {

        }

        protected override void HookIncomingCalls()
        {
            this._connection.On<VotronicData>(nameof(IDataHub.VotronicData), (data) =>
            {
                this.VotronicDataReceived?.Invoke(this, data);
            });

            this._connection.On<int, RuuviTagDeviceData>(nameof(IDataHub.RuuviTagData), (id, data) =>
            {
                this.RuuviTagDeviceDataReceived?.Invoke(this, (id, data));
            });
        }

        protected override Uri LoadUri()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> ShouldInitialize(CancellationToken cancellationToken) => Task.FromResult(true);

        protected override void SpecificReinitialize()
        {
            throw new NotImplementedException();
        }
    }
    public interface IDataHubClient : IBaseHubClient
    {
        event EventHandler<VotronicData> VotronicDataReceived;
        event EventHandler<(int, RuuviTagDeviceData)> RuuviTagDeviceDataReceived;
    }
}
