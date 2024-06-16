using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiSettingService : IMauiSettingService
    {
        public string Url { get; set; } = "https://chdcaravanui-christoph25.eu1.pitunnel.com/api/";

        public string GetDataHubUri(NavigationManager navigationManager = null) => this.Url;
    }

    public interface IMauiSettingService : ISettingService
    {
        string Url { get; set; }
    }
}
