using System;
using System.Collections.Generic;

namespace WeatherData.Models.Entity
{
    public partial class City
    {
        public City()
        {
            Temperatures = new HashSet<Temperature>();
        }

        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool? Relevant { get; set; }
        public DateTime LastRequestedDate { get; set; }

        public virtual ICollection<Temperature> Temperatures { get; set; }
    }
}
