using System;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Infrastructure.ApplicationInsights;
using NetCoreAngularApp.Template.Infrastructure.Identity;

namespace NetCoreAngularApp.Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool addApplicationInsights = true)
    {
        services.AddScoped<IUser, CurrentUser>();

        services.AddSingleton(TimeProvider.System);

        if (addApplicationInsights)
        {
            services.AddApplicationInsights(configuration);
        }

        return services;
    }

    private static IServiceCollection AddApplicationInsights(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var applicationInsightsSettings = new ApplicationInsightsSettings();
        configuration.Bind(
            ApplicationInsightsSettings.ApplicationInsightsSectionKey,
            applicationInsightsSettings);
        services.AddSingleton(applicationInsightsSettings);

        services.AddSingleton<ITelemetryInitializer, CloudRoleTelemetryInitializer>();

        services.AddApplicationInsightsTelemetry();

        services.Configure<TelemetryConfiguration>(tc =>
            tc.DisableTelemetry = applicationInsightsSettings.DisableTelemetry);

        var tiToRemove = services.FirstOrDefault
            (t => t.ImplementationType ==
                typeof(AspNetCoreEnvironmentTelemetryInitializer));

        if (tiToRemove is not null)
        {
            services.Remove(tiToRemove);
        }

        return services;
    }
}
