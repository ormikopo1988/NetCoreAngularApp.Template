using System;

namespace NetCoreAngularApp.Template.Application.WeatherForecasts.Dtos;

public class WeatherForecastDto
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
