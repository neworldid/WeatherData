using WeatherData.Models;
using WeatherData.Models.Entity;

namespace WeatherData.Contracts;

public interface IWeatherDataService
{
	List<City> GetLastRequestedCities(int count);
	
	void AddTemperatureData(IEnumerable<TemperatureRecord> records);
	
	City? GetExistingCity(string cityName, string countryCode);
	
	void UpdateCityData(int cityId);
	
	void AddCity(string cityName, string countryCode);
	
	IEnumerable<TemperatureRecordModel>? GetActualTemperatureData(IList<int> cityIds, int recordNumber);
}