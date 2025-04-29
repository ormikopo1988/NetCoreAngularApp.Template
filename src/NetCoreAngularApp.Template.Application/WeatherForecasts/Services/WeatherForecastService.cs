using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Mappings;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IApplicationDbContext _applicationDbContext;

    public WeatherForecastService(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<(int, IEnumerable<WeatherForecastDto>)>
        GetAllAsync(
            CancellationToken ct = default)
    {
        var weatherForecasts = await _applicationDbContext
            .WeatherForecasts
            .ToListAsync(ct);

        return (weatherForecasts.Count,
            weatherForecasts.ToWeatherForecastDtos());
    }
}
