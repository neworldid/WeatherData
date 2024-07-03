using System;
using System.Collections.Generic;

namespace WeatherData.Models.Entity
{
    public partial class City
    {
        public City()
        {
            TemperatureRecords = new HashSet<TemperatureRecord>();
        }

        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public DateTime LastRequestedDate { get; set; }

        public virtual ICollection<TemperatureRecord> TemperatureRecords { get; set; }
    }
}
