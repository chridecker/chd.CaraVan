using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.Contracts.Dtos
{
    public class RuuviSensorDataDto
    {
        public int Id { get; set; }
        public decimal? Value { get; set; }
        public DateTime Record { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
