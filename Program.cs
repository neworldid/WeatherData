using Microsoft.EntityFrameworkCore;
using WeatherData.BackgroundServices;
using WeatherData.Contracts;
using WeatherData.DataServices;
using WeatherData.Models;
using WeatherData.Models.Entity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<WeatherDataContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<WeatherDataBackgroundService>();
builder.Services.AddSingleton<IWeatherDataServiceProcessor, WeatherDataServiceProcessor>();
builder.Services.AddSingleton<IHttpClientFacade, HttpClientFacade>();
builder.Services.AddSingleton<IDateTimeFacade, DateTimeFacade>();
builder.Services.AddSingleton<IWeatherDataContextProvider, WeatherDataContextProvider>();
builder.Services.AddSingleton<IWeatherDataService, WeatherDataService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();