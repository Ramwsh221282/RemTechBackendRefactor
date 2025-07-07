using Microsoft.Extensions.Configuration;

namespace RemTech.ParserManagement.Cache.Adapter.Configuration;

public sealed class RedisConfiguration
{
    private readonly string _connectionString;

    public RedisConfiguration(string filePath)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(filePath).Build();
        IConfigurationSection section = configuration
            .GetSection(nameof(RedisConfiguration))
            .GetSection("ConnectionString");
        string? connectionString = section.Value;
        ArgumentNullException.ThrowIfNull(connectionString);
        _connectionString = connectionString;
    }

    public static implicit operator string(RedisConfiguration configuration) =>
        configuration._connectionString;

    public string ConnectionAsString() => _connectionString;
}
