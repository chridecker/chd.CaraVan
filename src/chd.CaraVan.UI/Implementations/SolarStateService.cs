using chd.CaraVan.Devices.Contracts.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class SolarStateService : ISolarStateService
    {
        public string GetState(decimal state) => (state) switch
        {
            SolarStateConstants.STANDBY_STATE => SolarStateConstants.STANDBY,
            SolarStateConstants.ACTIVE_STATE => SolarStateConstants.ACTIVE,
            SolarStateConstants.REDUCE_STATE => SolarStateConstants.REDUCE,
            SolarStateConstants.REDUCE_STATE2 => SolarStateConstants.REDUCE,
            _ => state.ToString()
        };
    }
    public interface ISolarStateService
    {
        string GetState(decimal state);
    }
}
