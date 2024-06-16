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
        public IEnumerable<string> Urls { get; set; } = Enumerable.Empty<string>();
        public string Url { get; set; }
    }
    public interface ISettingService
    {
        string GetDataHubUri(NavigationManager navigationManager = null);
        IEnumerable<string> Urls { get; set; }
        string Url { get; set; }
    }
}
