using System.Text.Json;
using WeatherData.Contracts;
using WeatherData.Models;
using WeatherData.Models.Entity;

namespace WeatherData.BackgroundServices;

public class WeatherDataServiceProcessor : IWeatherDataServiceProcessor
{
	private readonly IHttpClientFacade _client;
	private readonly IWeatherDataService _weatherDataService;
	private readonly IWeatherDataConfigurationProvider _configurationProvider;
	private readonly IDateTimeFacade _dateTime;

	public WeatherDataServiceProcessor(
		IHttpClientFacade client, 
		IWeatherDataService weatherDataService, 
		IWeatherDataConfigurationProvider configurationProvider, 
		IDateTimeFacade dateTime)
	{
		_client = client;
		_weatherDataService = weatherDataService;
		_configurationProvider = configurationProvider;
		_dateTime = dateTime;
	}
	
	public void UpdateTemperatureData()
	{
		var activeCities = _weatherDataService.GetLastRequestedCities(_configurationProvider.GetActualCitiesNumberLimit());
		var temperatureRecords = new List<TemperatureRecord>();
		
		foreach (var city in activeCities)
		{
			var weatherData = GetWeatherData(city.CityName);

			temperatureRecords.Add(new TemperatureRecord
			{
				CityId = city.Id,
				Temperature = weatherData.main.temp,
				ModifiedTime = _dateTime.Now()
			});
		}
		_weatherDataService.AddTemperatureData(temperatureRecords);
	}
	
	private WeatherModel? GetWeatherData(string cityName)
	{
		var data = _client.GetStringAsync(_configurationProvider.GetWeatherApiLink(cityName))
			.GetAwaiter().GetResult();
		
		return JsonSerializer.Deserialize<WeatherModel>(data);
	}
}