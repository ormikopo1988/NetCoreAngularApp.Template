using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreAngularApp.Template.Application.Common.Interfaces;
using NetCoreAngularApp.Template.Persistence.Interceptors;

namespace NetCoreAngularApp.Template.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection") ??
            throw new ArgumentNullException("DefaultConnection");

        return services
            .AddDatabase(connectionString)
            .AddHealthChecks(connectionString);
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) => options
                .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
                .UseNpgsql(connectionString: connectionString, npgsqlOptions => npgsqlOptions
                    .MigrationsAssembly("NetCoreAngularApp.Template.Migrations")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        return services;
    }

    private static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        string connectionString)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(connectionString: connectionString);

        return services;
    }
}
