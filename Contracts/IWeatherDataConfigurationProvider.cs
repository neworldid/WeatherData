namespace WeatherData.Contracts;

public interface IWeatherDataConfigurationProvider
{
	int GetActualCitiesNumberLimit();
	
	int GetUpdateDataIntervalInSeconds();

	string GetWeatherApiLink(string cityName);
}