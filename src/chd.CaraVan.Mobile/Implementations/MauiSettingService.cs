using chd.CaraVan.UI.Implementations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Mobile.Implementations
{
    public class MauiSettingService : ISettingService
    {
        public string Url { get; set; }

        public IEnumerable<string> Urls { get; set; } = new List<string>()
        {
            "https://chdcaravanui-christoph25.eu1.pitunnel.com/",
            "http://192.168.1.102/"
        };
        public MauiSettingService()
        {
            this.Url = this.Urls.Any() ? this.Urls.FirstOrDefault() : string.Empty;
        }

        public string GetDataHubUri(NavigationManager navigationManager = null) => this.Url;
    }

}
