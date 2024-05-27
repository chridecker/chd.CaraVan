using chd.CaraVan.Contracts.Dtos;
using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Hubs
{
    public class DataHub : Hub<IDataHub>
    {
    }

    public interface IDataHub
    {
        Task RuuviTagData(int id, RuuviTagDeviceData data);
        Task VotronicData(VotronicData data);
    }
}
