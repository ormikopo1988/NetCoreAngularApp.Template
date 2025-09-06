using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Mappings;
using NetCoreAngularApp.Template.Domain.Entities;
using Xunit;

namespace NetCoreAngularApp.Template.Application.Tests.Unit.WeatherForecasts.Mappings;

public class WeatherForecastExtensionsTests
{
    [Fact]
    public void ToWeatherForecastDtos_MapsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var forecasts = new List<WeatherForecast>
        {
            new() 
            {
                Id = id,
                Date = new DateOnly(2025, 9, 6),
                TemperatureC = 25,
                Summary = "Sunny"
            }
        };

        // Act
        var dtos = forecasts.ToWeatherForecastDtos(
            TestContext.Current.CancellationToken);

        // Assert
        dtos.Should().HaveCount(1);
        var dto = dtos[0];
        dto.Id.Should().Be(id);
        dto.Date.Should().Be(new DateOnly(2025, 9, 6));
        dto.TemperatureC.Should().Be(25);
        dto.Summary.Should().Be("Sunny");
    }

    [Fact]
    public void ToWeatherForecastDtos_WithNullInput_ReturnsEmptyList()
    {
        // Arrange
        List<WeatherForecast>? forecasts = null;

        // Act
        var dtos = forecasts!.ToWeatherForecastDtos(
            TestContext.Current.CancellationToken);

        // Assert
        dtos.Should().BeEmpty();
    }

    [Fact]
    public void ToWeatherForecastDtos_WithEmptyInput_ReturnsEmptyList()
    {
        // Arrange
        var forecasts = new List<WeatherForecast>();

        // Act
        var dtos = forecasts.ToWeatherForecastDtos(
            TestContext.Current.CancellationToken);

        // Assert
        dtos.Should().BeEmpty();
    }

    [Fact]
    public void ToWeatherForecastDtos_ThrowsIfCancelled()
    {
        // Arrange
        var forecasts = new List<WeatherForecast>
        {
            new() 
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2025, 9, 6),
                TemperatureC = 20,
                Summary = "Cloudy"
            }
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Action act = () => forecasts.ToWeatherForecastDtos(cts.Token);

        // Assert
        act.Should().Throw<OperationCanceledException>();
    }
}
