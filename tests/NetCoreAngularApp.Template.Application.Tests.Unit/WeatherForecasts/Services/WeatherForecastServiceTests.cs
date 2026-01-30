using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Application.Common.Models;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Services;
using NetCoreAngularApp.Template.Domain.Common;
using NetCoreAngularApp.Template.Domain.Constants;
using NSubstitute;

namespace NetCoreAngularApp.Template.Application.Tests.Unit.WeatherForecasts.Services;

public class WeatherForecastServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsErrorResult_WhenExceptionThrown()
    {
        // Arrange
        var dbContext = Substitute.For<IApplicationDbContext>();
        dbContext.WeatherForecasts.Returns(x => throw new Exception("DB error"));

        var service = new WeatherForecastService(
            NullLogger<WeatherForecastService>.Instance,
            dbContext);

        var queryOptions = new BasePaginationQuery { Page = 1, PageSize = 2 };

        // Act
        var result = await service.GetAllAsync(
            queryOptions,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.EventId.Should().Be(LogEventId.WeatherForecastServiceGetAllFailed);
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error!.Exception!.Message.Should().Contain("DB error");
        result.Error!.Message.Should().Contain("Unhandled error");
    }
}
