using WeatherData.Models.Entity;

namespace WeatherData.Contracts;

public interface IWeatherDataContextProvider
{
	WeatherDataContext GetContextScope();
}