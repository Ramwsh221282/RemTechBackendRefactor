using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using Remtech.Infrastructure.RabbitMQ;
using Remtech.Infrastructure.RabbitMQ.Consumers;
using RemTech.Tests.Shared;
using Serilog;
using Telemetry.Infrastructure.PostgreSQL.Repositories;
using Telemetry.WebApi;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Telemetry.Tests;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder().FormPgVectorContainer();

    private readonly RabbitMqContainer _rabbitContainer =
        new RabbitMqBuilder().FormRabbitMqContainer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        NpgsqlOptions pgOptions = _dbContainer.CreateNpgsqlOptions();
        RabbitMqOptions rabbitMqOptions = _dbContainer.CreateRabbitMqOptions();

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<NpgsqlOptions>();
            services.RemoveAll<RabbitMqOptions>();
            services.AddRabbitMqProvider();
            services.AddScoped<TelemetryServiceDbContext>();
            services.TryAddSingleton(pgOptions);
            services.TryAddSingleton(rabbitMqOptions);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitContainer.StartAsync();
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.GetService<TelemetryServiceDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _rabbitContainer.StopAsync();
    }
}
