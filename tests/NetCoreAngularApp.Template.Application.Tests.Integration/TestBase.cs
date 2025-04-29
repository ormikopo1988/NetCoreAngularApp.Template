using System;
using NetCoreAngularApp.Template.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreAngularApp.Template.Application.Tests.Integration;

public abstract class TestBase : IDisposable
{
    private readonly IServiceScope _scope;

    internal readonly ApplicationDbContext DbContext;

    protected readonly IServiceProvider Services;

    protected TestBase(NetCoreAngularAppTemplateFixture fixture)
    {
        _scope = fixture.CreateScope();

        Services = _scope.ServiceProvider;

        DbContext = Services.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
