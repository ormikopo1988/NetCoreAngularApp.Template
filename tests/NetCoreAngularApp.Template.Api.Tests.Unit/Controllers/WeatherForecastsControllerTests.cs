using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetCoreAngularApp.Template.Api.Controllers;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;
using NetCoreAngularApp.Template.Domain.Common;
using NSubstitute;

namespace NetCoreAngularApp.Template.Api.Tests.Unit.Controllers;

public class WeatherForecastsControllerTests
{
    private readonly WeatherForecastsController _sut;
    private readonly IWeatherForecastService _weatherForecastService = 
        Substitute.For<IWeatherForecastService>();
    private readonly List<WeatherForecastDto> _expectedWeatherForecastDtos = [
        new WeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(0)),
            TemperatureC = 10,
            Summary = "Cold"
        },
        new WeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            TemperatureC = 0,
            Summary = "Freezing"
        },
        new WeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            TemperatureC = 20,
            Summary = "Warm"
        }
    ];

    public WeatherForecastsControllerTests()
    {
        _sut = new WeatherForecastsController(_weatherForecastService);
    }

    [Fact]
    public async Task GetAll_ShouldReturnWeatherForecasts_WhenCalled()
    {
        // Arrange
        var basePaginationQuery = new BasePaginationQuery
        {
            Page = 1,
            PageSize = 10
        };
        
        var cancellationToken = TestContext.Current.CancellationToken;
        
        _weatherForecastService
            .GetAllAsync(basePaginationQuery, cancellationToken)
            .Returns(Results.Ok(
                new PaginatedList<WeatherForecastDto>
                {
                    Items = _expectedWeatherForecastDtos,
                    Page = basePaginationQuery.Page,
                    PageSize = basePaginationQuery.PageSize,
                    Total = 20
                }));

        // Act
        var response = await _sut
            .GetAll(basePaginationQuery, cancellationToken);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        var result = response.Result as OkObjectResult;
        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeOfType<PaginatedList<WeatherForecastDto>>();
        var value = result.Value as PaginatedList<WeatherForecastDto>;
        value.Should().NotBeNull();
        value!.Items.Should().BeEquivalentTo(_expectedWeatherForecastDtos);
    }
}
