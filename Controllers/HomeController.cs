using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Models;
using WeatherData.Models.Entity;

namespace WeatherData.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly HttpClient _client;
	private readonly WeatherDataContext _context;

	public HomeController(ILogger<HomeController> logger, HttpClient client, WeatherDataContext context)
	{
		_logger = logger;
		_client = client;
		_context = context;
	}

	public IActionResult Index()
	{
		/*var newCity = new City
		{
			CityName = "Moscow",
			Country = "Russia"
		};
		
		_context.Cities.Add(newCity);
		_context.SaveChanges();*/
		
		
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}