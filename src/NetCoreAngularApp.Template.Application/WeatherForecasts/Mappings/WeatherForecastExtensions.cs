using System.Collections.Generic;
using System.Linq;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Domain.Entities;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Mappings;

public static class WeatherForecastExtensions
{
    public static List<WeatherForecastDto> ToWeatherForecastDtos(
        this IEnumerable<WeatherForecast> weatherForecasts)
    {
        return [.. weatherForecasts
            .Select(wf => new WeatherForecastDto
            {
                Id = wf.Id,
                Date = wf.Date,
                TemperatureC = wf.TemperatureC,
                Summary = wf.Summary
            })];
    }
}
