using Cleaners.Adapter.Outbox;
using Cleaners.Adapter.Storage;
using Cleaners.Adapters.Cache;
using Cleaners.Domain.Cleaners.Ports.Cache;
using Cleaners.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration;
using RemTech.Shared.Configuration.Options;
using RemTech.Shared.Tests;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Cleaners.Tests;

public sealed class CleanersTestHostFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().BuildPgVectorContainer();

    private readonly RedisContainer _cacheContainer = new RedisBuilder().BuildRedisContainer();

    private readonly RabbitMqContainer _rabbitContainer =
        new RabbitMqBuilder().BuildRabbitMqContainer();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _cacheContainer.StartAsync();
        await _rabbitContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        await using var context = scope.GetService<CleanersDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _cacheContainer.StopAsync();
        await _rabbitContainer.StopAsync();

        await _dbContainer.DisposeAsync();
        await _cacheContainer.DisposeAsync();
        await _rabbitContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        var dbOptions = _dbContainer.CreateDatabaseConfiguration();
        var cacheOptions = _cacheContainer.CreateCacheOptions();
        var rabbitOptions = _rabbitContainer.CreateRabbitMqConfiguration();

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IOptions<DatabaseOptions>>();
            services.RemoveAll<IOptions<CacheOptions>>();
            services.RemoveAll<IOptions<RabbitMqOptions>>();
            services.RemoveAll<RabbitMqConnectionProvider>();
            services.RemoveAll<CleanersDbContext>();
            services.RemoveAll<ICleanersCachedStorage>();
            services.RemoveAll<CleanersCachedStorage>();
            services.RemoveAll<PostgresDatabase>();

            IOptions<DatabaseOptions> db = Options.Create(dbOptions);
            IOptions<CacheOptions> cache = Options.Create(cacheOptions);
            IOptions<RabbitMqOptions> rabbit = Options.Create(rabbitOptions);

            services.AddSingleton(db);
            services.AddSingleton(cache);
            services.AddSingleton(rabbit);

            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<PostgresDatabase>();
            services.AddScoped<CleanersDbContext>();
            services.AddScoped<ICleanersCachedStorage, CleanersCachedStorage>();
            services.AddCleanersOutboxProcessor();
        });
    }
}
