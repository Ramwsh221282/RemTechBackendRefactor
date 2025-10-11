using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.DependencyInjection;
using Remtech.Infrastructure.RabbitMQ;
using Remtech.Infrastructure.RabbitMQ.Consumers;
using Serilog;
using Telemetry.Infrastructure.PostgreSQL;
using Telemetry.Infrastructure.PostgreSQL.Repositories;
using Telemetry.WebApi;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Telemetry.Tests;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
        .WithDatabase("database")
        .WithUsername("username")
        .WithPassword("password")
        .Build();

    private readonly RabbitMqContainer _rabbitContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:management-alpine")
        .WithPassword("password")
        .WithUsername("username")
        .WithHostname("hostname")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        PostgreSqlConnectionOptions pgOptions = CreatePostgreSqlConnectionOptions();
        RabbitMqOptions rabbitMqOptions = CreateRabbitMqOptions();

        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TelemetryServiceDbContext>();
            services.RemoveAll<PostgreSqlConnectionOptions>();
            services.RemoveAll<RabbitMqOptions>();

            services.AddRabbitMqProvider();
            services.AddScoped<TelemetryServiceDbContext>();
            services.TryAddSingleton(pgOptions);
            services.TryAddSingleton(rabbitMqOptions);
            services.TryAddSingleton(logger);
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

    private PostgreSqlConnectionOptions CreatePostgreSqlConnectionOptions()
    {
        string connectionString = _dbContainer.GetConnectionString();
        string[] pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> parameters = [];
        foreach (string pair in pairs)
        {
            string[] keyValuePair = pair.Split('=');
            string optionName = keyValuePair[0];
            string optionValue = keyValuePair[1];
            parameters.Add(optionName, optionValue);
        }

        return new PostgreSqlConnectionOptions()
        {
            Hostname = parameters["Host"],
            Port = parameters["Port"],
            Username = parameters["Username"],
            Password = parameters["Password"],
            Database = parameters["Database"],
        };
    }

    private RabbitMqOptions CreateRabbitMqOptions()
    {
        string connectionString = _rabbitContainer.GetConnectionString();
        string[] parts = connectionString.Split('@', StringSplitOptions.RemoveEmptyEntries);
        string[] hostParts = parts[1].Split(':', StringSplitOptions.RemoveEmptyEntries);
        string host = hostParts[0];
        string port = hostParts[1].Replace("/", string.Empty);
        string[] userParts = parts[0]
            .Split("//", StringSplitOptions.RemoveEmptyEntries)[1]
            .Split(':', StringSplitOptions.RemoveEmptyEntries);
        string username = userParts[0];
        string password = userParts[1];
        return new RabbitMqOptions()
        {
            HostName = host,
            Port = port,
            Password = password,
            UserName = username,
        };
    }
}
