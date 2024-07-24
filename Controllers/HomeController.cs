using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Contracts;
using WeatherData.Models;

namespace WeatherData.Controllers;

public class HomeController : Controller
{
	private readonly IWeatherDataService _dataService;

	public HomeController(IWeatherDataService dataService)
	{
		_dataService = dataService;
	}

	public IActionResult Index()
	{
		return View();
	}
	
	[HttpGet]
	public Task<JsonResult> GetTemperatureRecords()
	{
		var cityIds = _dataService.GetLastRequestedCities(5).Select(x => x.Id).ToList();
		
		var temperatures = _dataService.GetActualTemperatureData(cityIds, 5);
		
		return Task.FromResult(Json(new { Success = true, Data = temperatures }));
	}

	[ExcludeFromCodeCoverage]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}