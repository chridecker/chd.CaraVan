using chd.CaraVan.Devices.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chd.CaraVan.Devices.Contracts.Dtos.RuvviTag
{
    public class RuuviTagData
    {
        [JsonPropertyName("data_format")]
        public ERuuviDataFormat DataFormat { get; set; }
        public decimal humidity { get; set; }
        public decimal pressure { get; set; }
        public decimal temperature { get; set; }
        public decimal tx_power { get; set; }
        public decimal battery { get; set; }
        public string Mac { get; set; }
    }
}
