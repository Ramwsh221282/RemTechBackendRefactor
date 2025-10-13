using RemTech.Infrastructure.PostgreSQL;
using Remtech.Infrastructure.RabbitMQ;
using Remtech.Infrastructure.RabbitMQ.Consumers;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace RemTech.Tests.Shared;

public static class DbContainerExtensions
{
    public static NpgsqlOptions CreateNpgsqlOptions(this PostgreSqlContainer container)
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

        return new NpgsqlOptions()
        {
            Hostname = parameters["Host"],
            Port = parameters["Port"],
            Username = parameters["Username"],
            Password = parameters["Password"],
            Database = parameters["Database"],
        };
    }

    public static RabbitMqOptions CreateRabbitMqOptions(this PostgreSqlContainer container)
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
            Hostname = host,
            Port = port,
            Password = password,
            Username = username,
        };
    }

    public static PostgreSqlContainer FormPgVectorContainer(this PostgreSqlBuilder builder) =>
        builder
            .WithImage("pgvector/pgvector:0.8.0-pg17-bookworm")
            .WithDatabase("database")
            .WithUsername("username")
            .WithPassword("password")
            .Build();

    public static RabbitMqContainer FormRabbitMqContainer(this RabbitMqBuilder builder) =>
        builder
            .WithImage("rabbitmq:management-alpine")
            .WithPassword("password")
            .WithUsername("username")
            .WithHostname("hostname")
            .Build();
}
