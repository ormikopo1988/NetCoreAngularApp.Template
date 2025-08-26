using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Domain.Entities;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Mappings;

public static class WeatherForecastExtensions
{
    public static List<WeatherForecastDto> ToWeatherForecastDtos(
        this IEnumerable<WeatherForecast> weatherForecasts, 
        CancellationToken ct = default)
    {
        var dtos = new List<WeatherForecastDto>();

        if (weatherForecasts is not null)
        {
            foreach (var wf in weatherForecasts)
            {
                ct.ThrowIfCancellationRequested();

                dtos.Add(new WeatherForecastDto
                {
                    Id = wf.Id,
                    Date = wf.Date,
                    TemperatureC = wf.TemperatureC,
                    Summary = wf.Summary
                });
            }
        }

        return dtos;
    }
}
