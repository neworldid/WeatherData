using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherData.Contracts;
using WeatherData.Controllers;
using WeatherData.Models;
using WeatherData.Models.Entity;
using Xunit;

namespace WeatherData.UnitTests;

public class HomeControllerTests
{
	private readonly Mock<IWeatherDataService> _dataServiceMock = new();
	private readonly Mock<IWeatherDataConfigurationProvider> _configurationMock = new();

	[Fact]
	public void Index_ReturnsViewResult()
	{
		// Arrange
		var controller = new HomeController(_dataServiceMock.Object, _configurationMock.Object);

		// Act
		var result = controller.Index();

		// Assert
		Assert.IsType<ViewResult>(result);
	}

	[Fact]
	public async Task GetTemperatureRecords_ReturnsJsonResultWithCorrectData()
	{
		// Arrange
		var cities = new List<City>
		{
			new() { CityName = "TestCity", LastRequestedDate = DateTime.Now, Id = 3}
		};

		var controller = new HomeController(_dataServiceMock.Object, _configurationMock.Object);

		_configurationMock.Setup(x => x.GetActualCitiesNumberLimit()).Returns(5);
		_dataServiceMock.Setup(ds => ds.GetLastRequestedCities(5)).Returns(cities);
		_dataServiceMock.Setup(ds => ds.GetActualTemperatureData(It.Is<List<int>>(x => x.Contains(3)), 5)).Returns(new List<TemperatureRecordModel>
		{
			new()
			{
				CityName = cities.First().CityName,
				Temperature = 25,
				ModifiedDate = DateTime.Now
			}
		});

		// Act
		var result = await controller.GetTemperatureRecords();

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		dynamic data = jsonResult.Value;
		Assert.True(data.Success);
		var dataList = (data.Data as IEnumerable<dynamic>).ToList();
		Assert.Equal("TestCity", dataList[0].CityName);
		_configurationMock.Verify(x => x.GetActualCitiesNumberLimit(), Times.Once);
		_dataServiceMock.Verify(ds => ds.GetLastRequestedCities(5), Times.Once);
		_dataServiceMock.Verify(ds => ds.GetActualTemperatureData(It.Is<List<int>>(x => x.Contains(3)), 5), Times.Once);
	}
}