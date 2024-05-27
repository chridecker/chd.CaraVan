using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Hubs.Clients.Base
{
    public abstract class BaseHubClient
    {

        protected readonly ILogger<BaseHubClient> _logger;
        protected volatile bool _isInitializing;
        protected HubConnection _connection;

        protected BaseHubClient(ILogger<BaseHubClient> logger)
        {
            this._logger = logger;
            this._isInitializing = false;
        }

        public Task StartAsync(CancellationToken cancellationToken = default) => this.ReInitialize(cancellationToken);

        protected abstract void SpecificReinitialize();
        protected abstract Task<bool> ShouldInitialize(CancellationToken cancellationToken);
        protected abstract void HookIncomingCalls();
        protected abstract Task DoInvokations(CancellationToken cancellationToken);

        protected abstract Uri LoadUri();

        protected async Task ReInitialize(CancellationToken cancellationToken = default)
        {
            if (this._connection != null)
            {
                this.SpecificReinitialize();

                this._connection.Closed -= Error;
                await this._connection.StopAsync();
                this._connection = null;
            }
            if (!this._isInitializing)
            {
                await this.Initialize();
            }
        }
        private async Task Initialize(CancellationToken cancellationToken = default)
        {
            if (!await this.ShouldInitialize(cancellationToken)) { return; }
            while (!this._isInitializing
                && (this._connection == null || this._connection.State != HubConnectionState.Connected))
            {
                this._isInitializing = true;
                try
                {
                    this._connection = new HubConnectionBuilder()
                         .WithAutomaticReconnect()
                         //.WithUrl(this.LoadUri())
                        .Build();

                    this._connection.Reconnecting += this.Error;
                    this._connection.Closed += this.Error;

                    this.HookIncomingCalls();

                    await _connection.StartAsync(cancellationToken);
                    await this.DoInvokations(cancellationToken);
                    break;
                }

                catch (Exception ex)
                {
                    this._logger?.LogError(ex, ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                finally
                {
                    this._isInitializing = false;
                }
            }
        }
        private Task Error(Exception? ex) => this.ReInitialize();
        public bool IsConnected => this._connection?.State == HubConnectionState.Connected;
    }

    public interface IBaseHubClient
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        bool IsConnected { get; }

    }
}
