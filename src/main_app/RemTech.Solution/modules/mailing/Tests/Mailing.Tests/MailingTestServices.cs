using Mailing.Adapters.Storage;
using Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;
using Mailing.Tests.CleanWriteTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using RemTech.Shared.Tests;
using Serilog;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Mailing.Tests;

public sealed class MailingTestServices : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().BuildPgVectorContainer();
    private readonly RedisContainer _cache = new RedisBuilder().BuildRedisContainer();
    private readonly Lazy<IServiceProvider> _services;
    private IServiceProvider Services => _services.Value;
    public MailingTestServices() => _services = new Lazy<IServiceProvider>(InitializeProvider);

    public AsyncServiceScope Scope() => Services.CreateAsyncScope();

    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        await _cache.StartAsync();
        ApplyDatabaseMigrations();
    }

    public async Task DisposeAsync()
    {
        await _db.StopAsync();
        await _cache.StopAsync();
        await _db.DisposeAsync();
        await _cache.DisposeAsync();
    }

    private void ApplyDatabaseMigrations()
    {
        Services.InvokeUpgrader(nameof(RemTech));
        Services.InvokeUpgrader(nameof(Mailing));
    }

    private IServiceProvider InitializeProvider()
    {
        ServiceCollection services = [];

        Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IOptions<DatabaseOptions> dbOptions = Options.Create(_db.CreateDatabaseConfiguration());
        IOptions<CacheOptions> cache = Options.Create(_cache.CreateCacheOptions());

        services.AddSingleton(dbOptions);
        services.AddSingleton(cache);
        services.AddSingleton(logger);

        services.AddStorageAdapter();
        services.AddRedis();
        services.AddPostgres();
        services.AddScoped<IPostmans, NpgSqlPostmans>();

        IServiceProvider provider = services.BuildServiceProvider();
        return provider;
    }
}