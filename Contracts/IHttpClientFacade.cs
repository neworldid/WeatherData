namespace WeatherData.Contracts;

public interface IHttpClientFacade
{
	Task<string> GetStringAsync(string requestUri);
	Task<HttpResponseMessage> GetAsync(string requestUri);
	Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
}