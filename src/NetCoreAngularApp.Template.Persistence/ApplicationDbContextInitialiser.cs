using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreAngularApp.Template.Domain.Entities;

namespace NetCoreAngularApp.Template.Persistence;

internal static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

internal class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");

            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");

            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        var currentItems = await _context.WeatherForecasts.AnyAsync();

        if (!currentItems)
        {
            _context.WeatherForecasts.AddRange(new List<WeatherForecast>
            {
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(0)),
                    TemperatureC = 20,
                    Summary = "Warm"
                },
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    TemperatureC = 10,
                    Summary = "Cold"
                },
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                    TemperatureC = 0,
                    Summary = "Freezing"
                },
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    TemperatureC = 30,
                    Summary = "Hot"
                },
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                    TemperatureC = 12,
                    Summary = "Chilly"
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}
