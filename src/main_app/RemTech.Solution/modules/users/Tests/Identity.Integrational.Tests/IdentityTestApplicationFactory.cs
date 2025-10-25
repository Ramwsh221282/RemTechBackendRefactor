using Identity.Adapter.Storage;
using Identity.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using RemTech.Shared.Tests;
using Shared.Infrastructure.Module.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Identity.Integrational.Tests;

public sealed class IdentityTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().BuildPgVectorContainer();

    private readonly RedisContainer _redisContainer = new RedisBuilder().BuildRedisContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        DatabaseOptions dbOptions = _dbContainer.CreateDatabaseConfiguration();
        CacheOptions cacheOptions = _redisContainer.CreateCacheOptions();

        builder.ConfigureTestServices(sp =>
        {
            sp.RemoveAll<IOptions<DatabaseOptions>>();
            sp.RemoveAll<IOptions<CacheOptions>>();
            sp.RemoveAll<IdentityDbContext>();

            IOptions<DatabaseOptions> options = Options.Create(dbOptions);
            IOptions<CacheOptions> cache = Options.Create(cacheOptions);

            sp.AddSingleton(options);
            sp.AddSingleton(cache);
            sp.AddScoped<IdentityDbContext>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        await using IdentityDbContext context = scope.GetService<IdentityDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
    }
}
