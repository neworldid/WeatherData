namespace WeatherData.Contracts;

public class HttpClientFacade : IHttpClientFacade
{
	private readonly HttpClient _httpClient;

	public HttpClientFacade(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<string> GetStringAsync(string requestUri)
	{
		return await _httpClient.GetStringAsync(requestUri);
	}

	public async Task<HttpResponseMessage> GetAsync(string requestUri)
	{
		return await _httpClient.GetAsync(requestUri);
	}

	public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
	{
		return await _httpClient.PostAsync(requestUri, content);
	}
}