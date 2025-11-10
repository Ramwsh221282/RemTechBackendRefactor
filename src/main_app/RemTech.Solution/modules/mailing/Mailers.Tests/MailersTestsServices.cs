using Mailers.Application;
using Mailers.Core;
using Mailers.Persistence.NpgSql;
using Microsoft.Extensions.Options;
using RemTech.Tests.Shared;
using Testcontainers.PostgreSql;
using MailersEncryptOptions = Mailers.Application.Configs.MailersEncryptOptions;

namespace Mailers.Tests;

public sealed class MailersTestsServices : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly Lazy<IServiceProvider> _sp;
    private IServiceProvider Sp => _sp.Value;

    public MailersTestsServices()
    {
        _sp = new Lazy<IServiceProvider>(InitializeServices);
    }

    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        ApplyDatabaseMigrations();
    }

    public async Task DisposeAsync()
    {
        await _db.StopAsync();
        await _db.DisposeAsync();
    }

    public AsyncServiceScope Scope()
    {
        return Sp.CreateAsyncScope();
    }

    private IServiceProvider InitializeServices()
    {
        ServiceCollection services = new();
        IOptions<NpgSqlOptions> dbOptions = Options.Create(_db.CreateDatabaseConfiguration());
        
        IOptions<MailersEncryptOptions> options = Options.Create(
            new MailersEncryptOptions() { Key = Guid.NewGuid().ToString() }
            );
        
        services.AddSingleton(dbOptions);
        services.AddPostgres();
        services.AddMailersPersistence();
        services.AddMailersCore();
        services.AddMailersApplication();
        services.AddSingleton(options);
        return services.BuildServiceProvider();
    }

    private void ApplyDatabaseMigrations()
    {
        var upgrader = Sp.GetRequiredKeyedService<IDbUpgrader>(nameof(MailersDbUpgrader));
        upgrader.ApplyMigrations();
    }
}