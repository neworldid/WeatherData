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

	[Fact]
	public void Index_ReturnsViewResult()
	{
		// Arrange
		var controller = new HomeController(_dataServiceMock.Object);

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

		var controller = new HomeController(_dataServiceMock.Object);
        
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
	}
}