using WeatherData.Contracts;

namespace WeatherData.Providers;

public class WeatherDataConfigurationProvider : IWeatherDataConfigurationProvider
{
	private readonly IConfiguration _configuration;

	public WeatherDataConfigurationProvider(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	
	public int GetActualCitiesNumberLimit()
	{
		if (!int.TryParse(_configuration["ActualCitiesNumberLimit"], out var recordNumber) || recordNumber < 0 || recordNumber > 5)
		{
			recordNumber = 5; //Default value
		}

		return recordNumber;
	}

	public int GetUpdateDataIntervalInSeconds()
	{
		if (!int.TryParse(_configuration["UpdateDataIntervalInSeconds"], out var intervalInSeconds) || intervalInSeconds < 5)
		{
			intervalInSeconds = 60; //Default value
		}

		return intervalInSeconds;
	}

	public string GetWeatherApiLink(string cityName)
	{
		return string.Format(
			_configuration["WeatherApiUrl"],
			cityName,
			_configuration["WeatherAppId"]);
	}
}