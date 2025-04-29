using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace NetCoreAngularApp.Template.Infrastructure.ApplicationInsights;

/// <summary>
/// Telemetry initializer to include details such like the Cloud Role Name and Cloud Role
/// Instance to every telemetry generated before it is being sent 
/// to Azure Application Insights resource.
/// </summary>
internal sealed class CloudRoleTelemetryInitializer : ITelemetryInitializer
{
    private readonly ApplicationInsightsSettings settings;

    private static readonly string MachineName =
        Environment.MachineName.ToLower(
            System.Globalization.CultureInfo.CurrentCulture);

    public CloudRoleTelemetryInitializer(ApplicationInsightsSettings settings)
    {
        this.settings = settings;
    }

    /// <summary>
    /// Initialize method will get called before the generated telemetry item is being sent to 
    /// the Application Insights resource in Azure Portal.
    /// </summary>
    /// <param name="telemetry"></param>
    public void Initialize(ITelemetry telemetry)
    {
        // Set custom role name here
        telemetry.Context.Cloud.RoleName = settings.CloudRoleName;

        // Set custom role instance here
        telemetry.Context.Cloud.RoleInstance = MachineName;
    }
}
