namespace WeatherData.Models;

public class WeatherModel
{
	public string name { get; set; }
	public Sys sys { get; set; }
	public Main main { get; set; }
}


public class Main
{
	public decimal temp { get; set; }
}

public class Sys
{
	public string country { get; set; }
}