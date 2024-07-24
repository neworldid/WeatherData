namespace WeatherData.Contracts;

public class DateTimeFacade: IDateTimeFacade
{
	public DateTime Now()
	{
		return DateTime.Now;
	}
}