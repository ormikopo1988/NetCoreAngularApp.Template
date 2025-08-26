using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Mappings;
using NetCoreAngularApp.Template.Domain.Common;
using NetCoreAngularApp.Template.Domain.Constants;
using NetCoreAngularApp.Template.Domain.Entities;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly ILogger<WeatherForecastService> _logger;
    private readonly IApplicationDbContext _applicationDbContext;

    public WeatherForecastService(
        ILogger<WeatherForecastService> logger,
        IApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IResult<PaginatedList<WeatherForecastDto>>>
        GetAllAsync(
            BasePaginationQuery queryOptions,
            CancellationToken ct = default)
    {
        List<WeatherForecast> weatherForecasts;
        int totalCount;

        try
        {
            var query = _applicationDbContext
                .WeatherForecasts
                .AsQueryable();

            totalCount = await query.CountAsync(ct);

            query = query
                .Skip((queryOptions.Page - 1) * queryOptions.PageSize)
                .Take(queryOptions.PageSize);

            weatherForecasts = await query
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                LogEventId.WeatherForecastServiceGetAllFailed,
                ex,
                "Unable to retrieve WeatherForecasts, error: {Error}",
                ex.Message);

            return Results.
                InternalServerError<PaginatedList<WeatherForecastDto>>(
                    ex, 
                    LogEventId.WeatherForecastServiceGetAllFailed);
        }

        return Results.Ok(
            new PaginatedList<WeatherForecastDto>
            {
                Items = weatherForecasts
                    .ToWeatherForecastDtos(ct),
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize,
                Total = totalCount
            });
    }
}
