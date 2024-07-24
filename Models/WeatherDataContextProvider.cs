using WeatherData.Contracts;
using WeatherData.Models.Entity;

namespace WeatherData.Models;

public class WeatherDataContextProvider : IWeatherDataContextProvider
{
	private readonly IServiceScopeFactory _scopeFactory;
	
	public WeatherDataContextProvider(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
	}
	
	public WeatherDataContext GetContextScope()
	{
		var scope = _scopeFactory.CreateScope();
		return scope.ServiceProvider.GetRequiredService<WeatherDataContext>();
	}
}