using Moq;
using WeatherData.BackgroundServices;
using WeatherData.Contracts;
using WeatherData.Models.Entity;
using Xunit;

namespace WeatherData.UnitTests;

public class WeatherDataServiceProcessorTests
{
	[Fact]
	public async Task UpdateTemperatureData_UpdatesDataForActiveCities()
	{
		// Arrange
		var httpClientMock = new Mock<IHttpClientFacade>();
		var weatherDataServiceMock = new Mock<IWeatherDataService>();
		var configurationMock = new Mock<IWeatherDataConfigurationProvider>();
		var dateTimeProviderMock = new Mock<IDateTimeFacade>();

		var cities = new List<City>
		{
			new() { CityName = "TestCity", LastRequestedDate = DateTime.Now }
		};

		const string weatherJson = "{\"main\": {\"temp\": 25}, \"name\": \"TestCity\"}";
	    
		var testTime = new DateTime(2024, 1, 1, 12, 0, 0);

		configurationMock.Setup(x => x.GetActualCitiesNumberLimit()).Returns(5);
		weatherDataServiceMock.Setup(ds => ds.GetLastRequestedCities(5)).Returns(cities);
		configurationMock.Setup(x => x.GetWeatherApiLink("TestCity")).Returns("https://api.openweathermap.org/data/2.5/weather");
		httpClientMock.Setup(client => client.GetStringAsync("https://api.openweathermap.org/data/2.5/weather")).ReturnsAsync(weatherJson);
		dateTimeProviderMock.Setup(time => time.Now()).Returns(testTime);

		var processor = new WeatherDataServiceProcessor(httpClientMock.Object,  weatherDataServiceMock.Object, configurationMock.Object, dateTimeProviderMock.Object);

		// Act
		await Task.Run(() => processor.UpdateTemperatureData());

		// Assert
		configurationMock.Verify(x => x.GetActualCitiesNumberLimit(), Times.Once);
		weatherDataServiceMock.Verify(ds => ds.GetLastRequestedCities(5), Times.Once);
		configurationMock.Verify(x => x.GetWeatherApiLink("TestCity"), Times.Once);
		httpClientMock.Verify(client => client.GetStringAsync("https://api.openweathermap.org/data/2.5/weather"), Times.Once);
		dateTimeProviderMock.Verify(time => time.Now(), Times.Once);;
		weatherDataServiceMock.Verify(ds =>
			ds.AddTemperatureData(It.Is<IEnumerable<TemperatureRecord>>(x => x.FirstOrDefault()!.Temperature == 25)), Times.AtLeastOnce);
	}
}