using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreAngularApp.Template.Api.Infrastructure;

namespace NetCoreAngularApp.Template.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var origins = configuration
                .GetSection("AllowCors").Get<string[]>() ?? [];

        services.AddCors(options => options.AddDefaultPolicy(
            policy => policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowCredentials()));

        services.AddControllers();

        services.AddOpenApi();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];

        if (builder.Environment.IsProduction()
            && !string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
