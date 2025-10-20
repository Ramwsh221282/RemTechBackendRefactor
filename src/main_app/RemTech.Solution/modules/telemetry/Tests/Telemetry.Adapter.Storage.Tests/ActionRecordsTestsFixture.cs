using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.Dapper;
using RemTech.Shared.Configuration;
using RemTech.Shared.Configuration.Options;
using RemTech.Shared.Tests;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.Adapter.Storage.Seeding;
using Telemetry.Adapter.Storage.Storage;
using Telemetry.Domain.Ports.Storage;
using Testcontainers.PostgreSql;

namespace Telemetry.Adapter.Storage.Tests;

public sealed class ActionRecordsTestsFixture : IAsyncLifetime
{
    private readonly Lazy<Task<IServiceProvider>> _serviceProvider;
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
        .WithDatabase("database")
        .WithUsername("username")
        .WithPassword("password")
        .Build();

    public ActionRecordsTestsFixture()
    {
        _serviceProvider = new Lazy<Task<IServiceProvider>>(InitializeServiceProvider);
    }

    private async Task<IServiceProvider> InitializeServiceProvider()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new VectorTypeHandler());

        IServiceCollection services = new ServiceCollection();
        await _dbContainer.StartAsync();
        DatabaseOptions options = _dbContainer.CreateDatabaseConfiguration();
        services.AddSingleton(options);
        OnnxEmbeddingGenerator.AddEmbeddingGenerator(services);
        services.AddScoped<ActionRecordDbContext>();
        services.AddScoped<IActionRecordsStorage, ActionRecordsStorage>();
        services.AddScoped<IActionRecordsSeeder, ActionRecordsSeeder>();
        await ApplyMigrations(options);
        return services.BuildServiceProvider();
    }

    public async Task InitializeAsync() => await _serviceProvider.Value;

    public async Task DisposeAsync() => await _dbContainer.StopAsync();

    private async Task ApplyMigrations(DatabaseOptions options)
    {
        await using ActionRecordDbContext context = new ActionRecordDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public AsyncServiceScope CreateScope()
    {
        if (!_serviceProvider.IsValueCreated)
            throw new InvalidOperationException(
                "ServiceProvider is not initialized. Ensure InitializeAsync() is called first."
            );
        return _serviceProvider.Value.Result.CreateAsyncScope();
    }
}
