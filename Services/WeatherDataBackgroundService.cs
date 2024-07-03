using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WeatherData.Models;
using WeatherData.Models.Entity;

namespace WeatherData.Services;

public class WeatherDataBackgroundService : BackgroundService
{
	private readonly ILogger<WeatherDataBackgroundService> _logger;
	private readonly HttpClient _client;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IConfiguration _configuration;
	private Timer _timer;

	public WeatherDataBackgroundService(ILogger<WeatherDataBackgroundService> logger, HttpClient client,
		IServiceScopeFactory scopeFactory, IConfiguration configuration)
	{
		_logger = logger;
		_client = client;
		_scopeFactory = scopeFactory;
		_configuration = configuration;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_configuration.GetValue<int>("UpdateDataIntervalInMinutes")));

		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(_configuration.GetValue<int>("UpdateDataIntervalInMinutes")), stoppingToken);
		}
	}
	
	private async void DoWork(object state)
	{
		_logger.LogInformation("WeatherDataBackgroundService is running.");

		using var scope = _scopeFactory.CreateScope();
		try
		{
			var context = scope.ServiceProvider.GetRequiredService<WeatherDataContext>();

			var activeCities = await context.Cities.OrderByDescending(x => x.LastRequestedDate).Take(5).ToListAsync();

			foreach (var city in activeCities)
			{
				var weatherData = GetWeatherData(city.CityName);

				var temperatureData = new TemperatureRecord()
				{
					City = city,
					Temperature = weatherData.main.temp,
					ModifiedTime = DateTime.Now
				};

				await context.TemperatureRecords.AddAsync(temperatureData);
				await context.SaveChangesAsync();
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}
	
	private WeatherModel? GetWeatherData(string cityName)
	{
		const string appId = "9b24294673a04bd5a32530f0895c841a";
		var data = _client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={appId}&units=metric").GetAwaiter().GetResult();
		return JsonSerializer.Deserialize<WeatherModel>(data);
	}

	public override Task StopAsync(CancellationToken stoppingToken)
	{
		_timer?.Change(Timeout.Infinite, 0);
		return base.StopAsync(stoppingToken);
	}

	public override void Dispose()
	{
		_timer?.Dispose();
		base.Dispose();
	}
}