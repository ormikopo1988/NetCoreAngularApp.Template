using System.Threading.Tasks;
using NetCoreAngularApp.Template.Application.WeatherForecasts.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreAngularApp.Template.Application.Tests.Integration.WeatherForecasts.Services;

public class WeatherForecastServiceTests : TestBase
{
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

        // Act
        var result = await _sut.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Item1.Should().Be(5);
        result.Item2.Should().NotBeEmpty();
        result.Item2.Should().HaveCount(5);
    }
}
