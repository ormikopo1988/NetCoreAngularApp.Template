using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;

namespace NetCoreAngularApp.Template.Application.Tests.Integration.WeatherForecasts.Services;

public class WeatherForecastServiceTests : TestBase, IAsyncLifetime
{
    private static readonly DateOnly today = 
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(0));
    private static readonly DateOnly tomorrow = 
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

    private readonly IWeatherForecastService _sut;

    public WeatherForecastServiceTests(NetCoreAngularAppTemplateFixture fixture)
        : base(fixture)
    {
        _sut = Services.GetRequiredService<IWeatherForecastService>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnWeatherForecasts_WhenCalled()
    {
        // Arrange
        DbContext
            .WeatherForecasts
            .Add(new()
                {
                    Date = today,
                    TemperatureC = 20,
                    Summary = "Warm"
                });

        DbContext
            .WeatherForecasts
            .Add(new()
                {
                    Date = tomorrow,
                    TemperatureC = 10,
                    Summary = "Cold"
                });

        await DbContext
            .SaveChangesAsync(
                TestContext.Current.CancellationToken);

        // Act
        var result = await _sut.GetAllAsync(
            new BasePaginationQuery
            {
                Page = 1,
                PageSize = 10
            },
            TestContext.Current.CancellationToken);

        // Assert
        result.Error.Should().BeNull();
        result.IsError.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.Total.Should().Be(2);
        result.Data.Items.Should().HaveCount(2);
        result.Data.HasNextPage.Should().BeFalse();
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);

        var sortedItems = result.Data.Items
            .OrderByDescending(x => x.Date)
            .ToList();

        var firstItem = sortedItems.FirstOrDefault();
        firstItem.Should().NotBeNull();
        firstItem.Date.Should().Be(tomorrow);
        firstItem.TemperatureC.Should().Be(10);
        firstItem.Summary.Should().Be("Cold");

        var lastItem = sortedItems.LastOrDefault();
        lastItem.Should().NotBeNull();
        lastItem.Date.Should().Be(today);
        lastItem.TemperatureC.Should().Be(20);
        lastItem.Summary.Should().Be("Warm");
    }

    [Fact]
    public async Task 
        GetAllAsync_ShouldReturnPaginatedWeatherForecasts_WhenItemsAreMoreThanThePaginatedRequestedOnes()
    {
        // Arrange
        DbContext
            .WeatherForecasts
            .Add(new()
                {
                    Date = today,
                    TemperatureC = 15,
                    Summary = "Neutral"
                });

        DbContext
            .WeatherForecasts
            .Add(new()
                {
                    Date = tomorrow,
                    TemperatureC = 0,
                    Summary = "Freezing"
                });

        await DbContext
            .SaveChangesAsync(
                TestContext.Current.CancellationToken);

        // Act
        var result = await _sut.GetAllAsync(
            new BasePaginationQuery
            {
                Page = 1,
                PageSize = 1
            },
            TestContext.Current.CancellationToken);

        // Assert
        result.Error.Should().BeNull();
        result.IsError.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.Total.Should().Be(2);
        result.Data.Items.Should().HaveCount(1);
        result.Data.HasNextPage.Should().BeTrue();
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(1);
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        DbContext
            .WeatherForecasts
            .RemoveRange(DbContext.WeatherForecasts);

        await DbContext
            .SaveChangesAsync(
                TestContext.Current.CancellationToken);
    }
}
