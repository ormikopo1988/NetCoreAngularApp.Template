using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Services;

namespace NetCoreAngularApp.Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        return services;
    }
}
