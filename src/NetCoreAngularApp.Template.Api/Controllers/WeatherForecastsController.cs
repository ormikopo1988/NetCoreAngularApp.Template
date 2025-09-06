using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreAngularApp.Template.Api.Extensions;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;

namespace NetCoreAngularApp.Template.Api.Controllers;

[ApiController]
[Route("api/weather-forecasts")]
public class WeatherForecastsController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastsController(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<WeatherForecastDto>>> 
        GetAll([FromQuery] BasePaginationQuery request, CancellationToken ct)
    {
        return await _weatherForecastService
            .GetAllAsync(request, ct)
            .AsActionResult();
    }
}
