using Microsoft.AspNetCore.Mvc;

namespace WeatherData.Controllers;

public class CitiesController : Controller
{
	// GET
	public IActionResult Index()
	{
		return View();
	}
}