using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;

public interface IWeatherForecastService
{
    Task<(int, IEnumerable<WeatherForecastDto>)>
        GetAllAsync(
            CancellationToken ct = default);
}
