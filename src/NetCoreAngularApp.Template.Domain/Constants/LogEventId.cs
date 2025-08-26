using Microsoft.Extensions.Logging;

namespace NetCoreAngularApp.Template.Domain.Constants;

public static class LogEventId
{
    #region WeatherForecastService

    public static readonly EventId WeatherForecastServiceGetAllFailed =
        new(10, nameof(WeatherForecastServiceGetAllFailed));

    #endregion
}
