using WeatherData.Contracts;
using WeatherData.Models;
using WeatherData.Models.Entity;

namespace WeatherData.DataServices;

public class WeatherDataService : IWeatherDataService
{
	private readonly IWeatherDataContextProvider _contextProvider;
	private readonly IDateTimeFacade _dateTimeFacade;
	
	public WeatherDataService(IWeatherDataContextProvider contextProvider, IDateTimeFacade dateTimeFacade)
	{
		_contextProvider = contextProvider;
		_dateTimeFacade = dateTimeFacade;
	}

	public List<City> GetLastRequestedCities(int count)
	{
		try
		{
			using var context = _contextProvider.GetContextScope();
			return context.Cities.OrderByDescending(x => x.LastRequestedDate).Take(count).ToList();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public void AddTemperatureData(IEnumerable<TemperatureRecord> records)
	{
		try
		{
			using var context = _contextProvider.GetContextScope();
			context.TemperatureRecords.AddRange(records);
			context.SaveChanges();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	public City? GetExistingCity(string cityName, string countryCode)
	{
		try
		{
			var context = _contextProvider.GetContextScope();
			return context.Cities.FirstOrDefault(x => x.CityName == cityName && x.Country == countryCode);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return null;
		}
	}

	public void UpdateCityData(int cityId)
	{
		try
		{
			var context = _contextProvider.GetContextScope();
			var city = context.Cities.Find(cityId);
			city.LastRequestedDate = _dateTimeFacade.Now();
			context.SaveChanges();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
	}

	public void AddCity(string cityName, string countryCode)
	{
		try
		{
			var cityEntity = new City()
			{
				CityName = cityName,
				Country = countryCode,
				LastRequestedDate = _dateTimeFacade.Now()
			};
				
			using var context = _contextProvider.GetContextScope();
			context.Cities.Add(cityEntity);
			context.SaveChanges();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	public IEnumerable<TemperatureRecordModel>? GetActualTemperatureData(IList<int> cityIds, int recordNumber)
	{
		try
		{
			using var context = _contextProvider.GetContextScope();
			var data = context.TemperatureRecords
				.Where(x => cityIds.Contains(x.CityId))
				.OrderByDescending(x => x.ModifiedTime)
				.Take(recordNumber).Select(x => new TemperatureRecordModel
				{
					CityName = x.City!.CityName,
					Country = x.City.Country,
					Temperature = x.Temperature,
					ModifiedDate = x.ModifiedTime
				}).ToList();
			return data;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return null;
		}
	}
}