namespace NetCoreAngularApp.Template.Infrastructure.ApplicationInsights;

internal sealed class ApplicationInsightsSettings
{
    public const string ApplicationInsightsSectionKey = "ApplicationInsights";

    public string CloudRoleName { get; set; } = default!;

    public bool DisableTelemetry { get; set; }
}
