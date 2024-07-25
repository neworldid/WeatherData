using WeatherData.Contracts;

namespace WeatherData.BackgroundServices;

public class WeatherDataBackgroundService : BackgroundService
{
	
	private readonly IWeatherDataConfigurationProvider _configurationProvider;
	private Timer _timer;
	private readonly IWeatherDataServiceProcessor _dataServiceProcessor;

	public WeatherDataBackgroundService(IWeatherDataConfigurationProvider configurationProvider, IWeatherDataServiceProcessor dataServiceProcessor)
	{
		_configurationProvider = configurationProvider;
		_dataServiceProcessor = dataServiceProcessor;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(_configurationProvider.GetUpdateDataIntervalInSeconds()));

		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(_configurationProvider.GetUpdateDataIntervalInSeconds()), stoppingToken);
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