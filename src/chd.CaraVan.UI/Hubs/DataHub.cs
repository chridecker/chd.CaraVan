using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Hubs
{
    public class DataHub : Hub<IDataHub>
    {
        private readonly ILogger<DataHub> _logger;

        public DataHub(ILogger<DataHub>logger)
        {
            this._logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            this._logger?.LogDebug($"Connected Client on Hub");
            return base.OnConnectedAsync();
        }
    }

    public interface IDataHub
    {
        Task RuuviTagData();
        Task VotronicData();
    }
}
