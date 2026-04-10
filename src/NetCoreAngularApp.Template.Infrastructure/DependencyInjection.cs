using System;
using Microsoft.Extensions.DependencyInjection;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Infrastructure.Identity;

namespace NetCoreAngularApp.Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUser, CurrentUser>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
