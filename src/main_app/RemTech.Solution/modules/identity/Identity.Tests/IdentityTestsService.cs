using Identity.Application;
using Identity.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;

namespace Identity.Tests;

public sealed class IdentityTestsService : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().BuildPgVectorContainer();
    
    private readonly Lazy<IServiceProvider> _sp;
    public IServiceProvider Sp => _sp.Value;
    public AsyncServiceScope Scope() => Sp.CreateAsyncScope();

    public IdentityTestsService()
    {
        _sp = new Lazy<IServiceProvider>(CreateServiceProvider);
    }

    private IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();
        IOptions<NpgSqlOptions> options = Options.Create(_container.CreateDatabaseConfiguration());
        services.AddSingleton(options);
        services.AddIdentityModule();
        return services.BuildServiceProvider();
    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Sp.ApplyIdentityPersistenceMigrations();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}