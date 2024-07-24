namespace WeatherData.Models;

public class TemperatureRecordModel
{
	public string CityName { get; set; } = null!;
	public string Country { get; set; } = null!;
	public decimal Temperature { get; set; }
	public DateTime ModifiedDate { get; set; }
}