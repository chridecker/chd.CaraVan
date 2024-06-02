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
        public string GetState(byte state) => (state, int.TryParse(state.ToString("X2").Substring(0, 1), out var d), d % 2) switch
        {
            (0, _, _) => SolarStateConstants.STANDBY,
            (_, true, 0) => SolarStateConstants.ACTIVE,
            (_, true, 1) => SolarStateConstants.REDUCE,
            _ => state.ToString()
        };
    }
    public interface ISolarStateService
    {
        string GetState(byte state);
    }
}
