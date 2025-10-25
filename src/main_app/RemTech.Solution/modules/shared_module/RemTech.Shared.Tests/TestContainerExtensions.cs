using RemTech.Shared.Configuration;
using RemTech.Shared.Configuration.Options;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace RemTech.Shared.Tests;

public static class TestContainerExtensions
{
    public static PostgreSqlContainer BuildPgVectorContainer(this PostgreSqlBuilder builder) =>
        builder
            .WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
            .WithDatabase("database")
            .WithUsername("username")
            .WithPassword("password")
            .Build();

    public static RedisContainer BuildRedisContainer(this RedisBuilder builder) =>
        builder.WithImage("redis:latest").Build();

    public static CacheOptions CreateCacheOptions(this RedisContainer container) =>
        new() { Host = container.GetConnectionString() };

    public static DatabaseOptions CreateDatabaseConfiguration(this PostgreSqlContainer container)
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

        return new DatabaseOptions()
        {
            Host = hostname,
            Port = port,
            Username = username,
            Password = password,
            Database = database,
        };
    }

    public static RabbitMqOptions CreateRabbitMqConfiguration(this RabbitMqContainer container)
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

        return new RabbitMqOptions()
        {
            HostName = host,
            Port = port,
            Password = password,
            UserName = username,
        };
    }
}
