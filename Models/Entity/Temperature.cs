using System;
using System.Collections.Generic;

namespace WeatherData.Models.Entity
{
    public partial class Temperature
    {
        public int Id { get; set; }
        public int? CityId { get; set; }
        public decimal Temperature1 { get; set; }
        public DateTime ModifiedTime { get; set; }

        public virtual City? City { get; set; }
    }
}
