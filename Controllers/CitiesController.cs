using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Contracts;
using WeatherData.Models;

namespace WeatherData.Controllers;

public class CitiesController : Controller
{
	private readonly IHttpClientFacade _client;
	private readonly IConfiguration _configuration;
	private readonly IWeatherDataService _weatherDataService;
	private readonly IWeatherDataServiceProcessor _weatherDataServiceProcessor;
	
	public CitiesController(IHttpClientFacade client, IConfiguration configuration, IWeatherDataService weatherDataService, IWeatherDataServiceProcessor weatherDataServiceProcessor)
	{
		_client = client;
		_configuration = configuration;
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
			var response = await _client.GetAsync(string.Format(
				_configuration["WeatherApiUrl"], 
				city.CityName,
				_configuration["WeatherAppId"]));
			
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
		var cities = _weatherDataService.GetLastRequestedCities(5);
		return Task.FromResult(Json(new { Success = true, Data = cities }));
	}
	
	[HttpGet]
	public IActionResult GetInterval()
	{
		try
		{
			var updateInterval = _configuration["UpdateDataIntervalInSeconds"];
			return Ok(updateInterval);
		}
		catch (Exception e)
		{
			return StatusCode(500);
		}
	}
}