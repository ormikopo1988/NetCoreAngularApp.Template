using System.Collections.Generic;
using System.Threading.Tasks;
using NetCoreAngularApp.Template.Infrastructure;
using NetCoreAngularApp.Template.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace NetCoreAngularApp.Template.Application.Tests.Integration;

public class NetCoreAngularAppTemplateFixture : IAsyncLifetime
{
    private IHost _host = default!;

    private readonly NetCoreAngularAppDbContainer _dbContainer = new();

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _host = ConfigureAppBuilder();

        using var scope = _host.Services.CreateScope();

        var initialiser = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }

    public IServiceScope CreateScope()
    {
        return _host.Services.CreateScope();
    }

    private IHost ConfigureAppBuilder()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.ConnectionString
            });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, false);
        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.RemoveAll<IHostedService>();

        return builder.Build();
    }

    public async ValueTask DisposeAsync()
    {
        _host?.Dispose();

        await _dbContainer.DisposeAsync();
    }
}
