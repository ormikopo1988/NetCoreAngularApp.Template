using System.Threading;
using System.Threading.Tasks;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Domain.Common;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;

public interface IWeatherForecastService
{
    Task<IResult<PaginatedList<WeatherForecastDto>>>
        GetAllAsync(
            BasePaginationQuery queryOptions,
            CancellationToken ct = default);
}
