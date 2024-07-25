using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Contracts;
using WeatherData.Models;

namespace WeatherData.Controllers;

public class CitiesController : Controller
{
	private readonly IHttpClientFacade _client;
	private readonly IWeatherDataConfigurationProvider _configurationProvider;
	private readonly IWeatherDataService _weatherDataService;
	private readonly IWeatherDataServiceProcessor _weatherDataServiceProcessor;
	
	public CitiesController(IHttpClientFacade client, IWeatherDataConfigurationProvider configurationProvider, IWeatherDataService weatherDataService, IWeatherDataServiceProcessor weatherDataServiceProcessor)
	{
		_client = client;
		_configurationProvider = configurationProvider;
		_weatherDataService = weatherDataService;
		_weatherDataServiceProcessor = weatherDataServiceProcessor;
	}
	
	public IActionResult Index()
	{
		return View();
	}
	
	[HttpPost]
	public async Task<JsonResult> AddCity([FromBody]CityRequest city)
	{
		try
		{
			var response = await _client.GetAsync(_configurationProvider.GetWeatherApiLink(city.CityName));
			
			if (!response.IsSuccessStatusCode)
				return Json(new { Success = false, Message = $"Impossible to get weather data for {city.CityName}" });
			
			var data = await response.Content.ReadAsStringAsync();
			var model = JsonSerializer.Deserialize<WeatherModel>(data);
			
			var existingCity = _weatherDataService.GetExistingCity(city.CityName, model.sys.country);
			if (existingCity != null)
			{
				_weatherDataService.UpdateCityData(existingCity.Id);
			}
			else
			{
				_weatherDataService.AddCity(city.CityName, model.sys.country);
			}
			
			_weatherDataServiceProcessor.UpdateTemperatureData();
			
			return Json(new { Success = true});
		}
		catch (Exception e)
		{
			return Json(new { Success = false, Message = $"Impossible to get weather data for {city.CityName}, {e.Message}" });
		}
	}
	
	[HttpGet]
	public Task<JsonResult> GetActualCities()
	{
		var cities = _weatherDataService.GetLastRequestedCities(_configurationProvider.GetActualCitiesNumberLimit());
		return Task.FromResult(Json(new { Success = true, Data = cities }));
	}
	
	[HttpGet]
	public IActionResult GetInterval()
	{
		var updateInterval = _configurationProvider.GetUpdateDataIntervalInSeconds();
		return Ok(updateInterval);
	}
}