using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace RemTech.Tests.Shared;

public static class TestProjectExtensions
{
    public static void ReRegisterAppsettingsJsonConfiguration(this IServiceCollection services)
    {
        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        services.RemoveAll<IConfigurationRoot>();
        services.AddSingleton(configurationRoot);
    }
    
    public static void ReRegisterNpgSqlOptions(this IServiceCollection services, PostgreSqlContainer container)
    {
        services.RemoveAll<IOptions<NpgSqlOptions>>();
        IOptions<NpgSqlOptions> options = Options.Create(container.CreateDatabaseConfiguration());
        services.AddSingleton(options);
    }

    public static void ReRegisterRabbitMqOptions(this IServiceCollection services, RabbitMqContainer container)
    {
        services.RemoveAll<IOptions<RabbitMqConnectionOptions>>();
        IOptions<RabbitMqConnectionOptions> options = Options.Create(container.CreateRabbitMqConfiguration());
        services.AddSingleton(options);
    }

    public static void ReRegisterBackgroundService<T>(this IServiceCollection services) 
        where T : class, IHostedService
    {
        ServiceDescriptor? registeredService = services.FirstOrDefault(s => s.ServiceType == typeof(T));
        if (registeredService == null) 
            throw new InvalidOperationException($"Cannot find registered service for {typeof(T)} when re registering background service.");
        services.RemoveAll(registeredService.ServiceType);
        services.AddHostedService<T>();
    }
    
    public static PostgreSqlContainer BuildPgVectorContainer(this PostgreSqlBuilder builder) =>
        builder
            .WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
            .WithDatabase("database")
            .WithUsername("username")
            .WithPassword("password")
            .Build();

    public static RabbitMqContainer BuildRabbitMqContainer(this RabbitMqBuilder builder) =>
        builder.WithImage("rabbitmq:3.11").Build();
    
    // public static RedisContainer BuildRedisContainer(this RedisBuilder builder) =>
    //     builder.WithImage("redis:latest").Build();
    //
    // public static CacheOptions CreateCacheOptions(this RedisContainer container) =>
    //     new() { Host = container.GetConnectionString() };
    //

    public static NpgSqlOptions CreateDatabaseConfiguration(this PostgreSqlContainer container)
    {
        string connectionString = container.GetConnectionString();
        string[] pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> parameters = [];

        foreach (string pair in pairs)
        {
            string[] keyValuePair = pair.Split('=');
            string optionName = keyValuePair[0];
            string optionValue = keyValuePair[1];
            parameters.Add(optionName, optionValue);
        }

        string hostname = parameters["Host"];
        string port = parameters["Port"];
        string username = parameters["Username"];
        string password = parameters["Password"];
        string database = parameters["Database"];

        return new NpgSqlOptions()
        {
            Host = hostname,
            Port = port,
            Username = username,
            Password = password,
            Database = database,
        };
    }

    public static RabbitMqConnectionOptions CreateRabbitMqConfiguration(this RabbitMqContainer container)
    {
        string connectionString = container.GetConnectionString();
        string[] parts = connectionString.Split('@', StringSplitOptions.RemoveEmptyEntries);
        string[] hostParts = parts[1].Split(':', StringSplitOptions.RemoveEmptyEntries);
    
        string host = hostParts[0];
        string port = hostParts[1].Replace("/", string.Empty);
        string[] userParts = parts[0]
            .Split("//", StringSplitOptions.RemoveEmptyEntries)[1]
            .Split(':', StringSplitOptions.RemoveEmptyEntries);
        string username = userParts[0];
        string password = userParts[1];
    
        return new RabbitMqConnectionOptions()
        {
            Hostname = host,
            Port = int.Parse(port),
            Password = password,
            Username = username,
        };
    }
}