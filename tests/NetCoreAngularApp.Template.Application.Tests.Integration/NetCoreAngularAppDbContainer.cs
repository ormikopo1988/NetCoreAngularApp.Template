using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace NetCoreAngularApp.Template.Application.Tests.Integration;

internal sealed class NetCoreAngularAppDbContainer
{
    private const string Database = "NetCoreAngularAppTemplateDb";
    private const string Username = "postgres";
    private const string Password = "postgres";
    private const ushort PgSqlPort = 5432;

    private readonly IContainer _pgsqlContainer;

    internal string ConnectionString =>
        $"Host={_pgsqlContainer.Hostname};Port={_pgsqlContainer.GetMappedPublicPort(PgSqlPort)};Database={Database};Username={Username};Password={Password};Include Error Detail=true";

    public NetCoreAngularAppDbContainer()
    {
        _pgsqlContainer = new ContainerBuilder("postgres:17")
            .WithPortBinding(PgSqlPort, true)
            .WithEnvironment("POSTGRES_DB", Database)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(PgSqlPort)
                .UntilMessageIsLogged(
                "database system is ready to accept connections",
                waitStrategy => waitStrategy.WithTimeout(TimeSpan.FromMinutes(5))))
            .Build();
    }

    public async Task StartAsync()
    {
        await _pgsqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _pgsqlContainer.DisposeAsync();
    }
}
