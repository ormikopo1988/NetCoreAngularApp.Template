using System;
using NetCoreAngularApp.Template.Domain.Common;

namespace NetCoreAngularApp.Template.Domain.Entities;

public class WeatherForecast : BaseEntity
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
