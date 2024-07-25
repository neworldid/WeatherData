using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherData.Contracts;
using WeatherData.Controllers;
using WeatherData.Models;
using WeatherData.Models.Entity;
using Xunit;

namespace WeatherData.UnitTests;

public class CitiesControllerTests
{
	private readonly Mock<IWeatherDataConfigurationProvider> _configurationMock = new();
	private readonly Mock<IHttpClientFacade> _httpClientMock = new();
	private readonly Mock<IWeatherDataService> _mockDataService = new();
	private readonly Mock<IWeatherDataServiceProcessor> _mockDataServiceProcessor = new();

	[Fact]
	public void Index_ReturnsViewResult()
	{
		// Arrange
		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = controller.Index();

		// Assert
		Assert.IsType<ViewResult>(result);
	}
    
	[Fact]
	public async Task AddCity_ThrowsException_WhenGetsConfigData()
	{
		// Arrange
		_configurationMock.Setup(x => x.GetWeatherApiLink("TestCity")).Throws(new Exception("Configuration error test message"));

		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = await controller.AddCity(new CityRequest { CityName = "TestCity" });

		// Assert
		dynamic data = result.Value;
		Assert.False(data.Success);
		Assert.Equal("Impossible to get weather data for TestCity, Configuration error test message", data.Message);
	}
    
	[Fact]
	public async Task AddCity_ReturnsFailure_WhenApiCallFails()
	{
		// Arrange
		var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
		
		_configurationMock.Setup(x => x.GetWeatherApiLink("TestCity")).Returns("https://api.openweathermap.org/data/2.5/weather");
		_httpClientMock.Setup(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather")).ReturnsAsync(responseMessage);

		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = await controller.AddCity(new CityRequest { CityName = "TestCity" });

		// Assert
		dynamic data = result.Value;
		Assert.False(data.Success);
		Assert.Equal("Impossible to get weather data for TestCity", data.Message);
		_configurationMock.Verify(x => x.GetWeatherApiLink("TestCity"), Times.Once);
		_httpClientMock.Verify(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather"), Times.Once);

	}
    
	[Fact]
	public async Task AddCity_ReturnsSuccess_WhenApiCallIsSuccessful()
	{
		// Arrange
		var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent("{\"sys\": {\"country\": \"US\"}, \"cityName\": \"TestCity\"}", Encoding.UTF8, "application/json")
		};
        
        
		_configurationMock.Setup(x => x.GetWeatherApiLink("TestCity")).Returns("https://api.openweathermap.org/data/2.5/weather");
		_httpClientMock.Setup(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather")).ReturnsAsync(responseMessage);
		_mockDataService.Setup(ds => ds.GetExistingCity("TestCity", "US")).Returns((City?)null);

		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = await controller.AddCity(new CityRequest { CityName = "TestCity" });

		// Assert
		dynamic data = result.Value;
		Assert.True(data.Success);
		_configurationMock.Verify(x => x.GetWeatherApiLink("TestCity"), Times.Once);
		_httpClientMock.Verify(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather"), Times.Once);
		_mockDataService.Verify(ds => ds.GetExistingCity("TestCity", "US"), Times.Once);
		_mockDataService.Verify(ds => ds.AddCity("TestCity", "US"), Times.Once);
		_mockDataServiceProcessor.Verify(ds => ds.UpdateTemperatureData(), Times.Once);
	}
    
	[Fact]
	public async Task AddCity_ReturnsSuccess_WhenApiCallIsSuccessfulAndCityAlreadyExistsInDB()
	{
		// Arrange
		var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent("{\"sys\": {\"country\": \"US\"}, \"cityName\": \"TestCity\"}", Encoding.UTF8, "application/json")
		};
        
        
		_configurationMock.Setup(x => x.GetWeatherApiLink("TestCity")).Returns("https://api.openweathermap.org/data/2.5/weather");
		_httpClientMock.Setup(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather")).ReturnsAsync(responseMessage);
		_mockDataService.Setup(ds => ds.GetExistingCity("TestCity", "US")).Returns(new City { CityName = "TestCity", Country = "US", Id = 33});

		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = await controller.AddCity(new CityRequest { CityName = "TestCity" });

		// Assert
		dynamic data = result.Value;
		Assert.True(data.Success);
		_configurationMock.Verify(x => x.GetWeatherApiLink("TestCity"), Times.Once);
		_httpClientMock.Verify(client => client.GetAsync("https://api.openweathermap.org/data/2.5/weather"), Times.Once);
		_mockDataService.Verify(ds => ds.GetExistingCity("TestCity", "US"), Times.Once);
		_mockDataService.Verify(ds => ds.UpdateCityData(33), Times.Once);
		_mockDataServiceProcessor.Verify(ds => ds.UpdateTemperatureData(), Times.Once);
	}

	[Fact]
	public async Task GetActualCities_ReturnsSuccess()
	{
		// Arrange
		var cities = new List<City>
		{
			new() { CityName = "TestCity", LastRequestedDate = DateTime.Now }
		};

		_configurationMock.Setup(x => x.GetActualCitiesNumberLimit()).Returns(6);
		_mockDataService.Setup(x => x.GetLastRequestedCities(6)).Returns(cities);
        
		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = await controller.GetActualCities();

		// Assert
		dynamic data = result.Value;
		Assert.True(data.Success);
		Assert.Equal(data.Data[0].CityName, "TestCity");
		_configurationMock.Verify(x => x.GetActualCitiesNumberLimit(), Times.Once);
		_mockDataService.Verify(ds => ds.GetLastRequestedCities(6), Times.Once);
	}

	[Fact]
	public void GetInterval_ReturnsCorrectInterval()
	{
		// Arrange
		_configurationMock.Setup(x => x.GetUpdateDataIntervalInSeconds()).Returns(60);
        
		var controller = new CitiesController(_httpClientMock.Object, _configurationMock.Object, _mockDataService.Object, _mockDataServiceProcessor.Object);

		// Act
		var result = controller.GetInterval();

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(60, okResult.Value);
	}
}