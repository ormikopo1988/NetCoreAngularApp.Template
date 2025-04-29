using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreAngularApp.Template.Domain.Entities;

namespace NetCoreAngularApp.Template.Persistence.Configurations;

internal sealed class WeatherForecastConfiguration
    : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        builder
            .Ignore(wf => wf.TemperatureF);
    }
}
