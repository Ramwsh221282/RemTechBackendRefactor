using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ParsedAdvertisements.Adapters.Messaging;
using ParsedAdvertisements.Adapters.Outbox;
using ParsedAdvertisements.Adapters.Storage;
using ParsedAdvertisements.Domain;
using RemTech.Shared.Tests;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Quartz;
using Shared.Infrastructure.Module.RabbitMq;
using Shared.Infrastructure.Module.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace ParsedAdvertisements.Tests;

public sealed class ParsedAdvertisementsServices : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().BuildPgVectorContainer();

    private readonly RabbitMqContainer _rabbitMqContainer =
        new RabbitMqBuilder().BuildRabbitMqContainer();

    private readonly RedisContainer _redisContainer = new RedisBuilder().BuildRedisContainer();

    private static IServiceProvider? _lazyInitialized;

    private Lazy<IServiceProvider> _sp;

    public ParsedAdvertisementsServices()
    {
        _sp = new Lazy<IServiceProvider>(() => PrepareServices());
    }

    public IServiceProvider Services => _sp.Value;

    public AsyncServiceScope CreateScope() => _sp.Value.CreateAsyncScope();

    private IServiceProvider PrepareServices()
    {
        if (_lazyInitialized != null)
            return _lazyInitialized;

        ServiceCollection services = new ServiceCollection();
        var dbOptions = Options.Create(_dbContainer.CreateDatabaseConfiguration());
        var rabbitOptions = Options.Create(_rabbitMqContainer.CreateRabbitMqConfiguration());
        var redisContainer = Options.Create(_redisContainer.CreateCacheOptions());

        services.AddSingleton(dbOptions);
        services.AddSingleton(rabbitOptions);
        services.AddSingleton(redisContainer);

        services.AddPostgres();
        services.AddRabbitMq();
        services.AddRedis();

        services.AddParsedAdvertisementsDomain();
        services.AddParsedAdvertisementsStorageAdapter();
        services.AddParsedAdvertisementsOutboxAdapter();
        services.AddParsedAdvertisementsMessagingAdapter();
        services.ConfigureQuartzScheduler();

        _lazyInitialized = services.BuildServiceProvider();
        return _lazyInitialized;
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _redisContainer.StartAsync();

        await EnsureDatabasesDeleted(typeof(ParsedAdvertisementsDbContext));

        await EnsureDatabasesCreated(
            typeof(ParsedAdvertisementsOutboxDbContext),
            typeof(ParsedAdvertisementsDbContext)
        );
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
        await _redisContainer.StopAsync();

        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }

    private async Task EnsureDatabasesDeleted(params Type[] types)
    {
        foreach (var type in types)
        {
            await EnsureDatabaseDeleted(type);
        }
    }

    private async Task EnsureDatabasesCreated(params Type[] types)
    {
        foreach (var type in types)
        {
            await EnsureDatabaseCreated(type);
        }
    }

    private async Task EnsureDatabaseDeleted<TContext>()
        where TContext : DbContext
    {
        Type type = typeof(TContext);
        await EnsureDatabaseDeleted(type);
    }

    private async Task EnsureDatabaseCreated<TContext>()
        where TContext : DbContext
    {
        Type type = typeof(TContext);
        await EnsureDatabaseCreated(type);
    }

    private async Task EnsureDatabaseDeleted(Type type)
    {
        await using var scope = _sp.Value.CreateAsyncScope();
        await using var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(type)!;
        try
        {
            await dbContext.Database.EnsureDeletedAsync();
        }
        catch { }
    }

    private async Task EnsureDatabaseCreated(Type type)
    {
        await using var scope = _sp.Value.CreateAsyncScope();
        await using var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(type)!;
        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch { }
    }
}
