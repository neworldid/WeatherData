using WeatherData.Contracts;

namespace WeatherData.BackgroundServices;

public class WeatherDataBackgroundService : BackgroundService
{
	
	private readonly IConfiguration _configuration;
	private Timer _timer;
	private readonly IWeatherDataServiceProcessor _dataServiceProcessor;

	public WeatherDataBackgroundService(IConfiguration configuration, IWeatherDataServiceProcessor dataServiceProcessor)
	{
		_configuration = configuration;
		_dataServiceProcessor = dataServiceProcessor;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(_configuration.GetValue<int>("UpdateDataIntervalInSeconds")));

		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(_configuration.GetValue<int>("UpdateDataIntervalInSeconds")), stoppingToken);
		}

		return;

		void TimerCallback(object? state)
		{
			_dataServiceProcessor.UpdateTemperatureData();
		}
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