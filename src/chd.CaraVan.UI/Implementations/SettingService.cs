using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class SettingService : ISettingService
    {
        public string GetDataHubUri(NavigationManager navigationManager = null) => navigationManager.BaseUri;
    }
    public interface ISettingService
    {
        string GetDataHubUri(NavigationManager navigationManager = null);
    }
}
