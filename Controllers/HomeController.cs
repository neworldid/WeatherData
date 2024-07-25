using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Contracts;
using WeatherData.Models;

namespace WeatherData.Controllers;

public class HomeController : Controller
{
	private readonly IWeatherDataService _dataService;
	private readonly IWeatherDataConfigurationProvider _configurationProvider;

	public HomeController(IWeatherDataService dataService, IWeatherDataConfigurationProvider configurationProvider)
	{
		_dataService = dataService;
		_configurationProvider = configurationProvider;
	}

	public IActionResult Index()
	{
		return View();
	}
	
	[HttpGet]
	public Task<JsonResult> GetTemperatureRecords()
	{
		var cityIds = _dataService.GetLastRequestedCities(5).Select(x => x.Id).ToList();
		
		var temperatures = _dataService.GetActualTemperatureData(cityIds, _configurationProvider.GetActualCitiesNumberLimit());
		
		return Task.FromResult(Json(new { Success = true, Data = temperatures }));
	}

	[ExcludeFromCodeCoverage]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}