using Microsoft.Extensions.Configuration;

namespace RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

public sealed class DatabaseConfiguration
{
    public string ConnectionString { get; }

    public DatabaseConfiguration(string jsonFilePath)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(jsonFilePath).Build();
        IConfigurationSection section = configuration
            .GetSection(nameof(DatabaseConfiguration))
            .GetSection(nameof(ConnectionString));
        string? connectionString = section.Value;
        ArgumentNullException.ThrowIfNull(connectionString);
        ConnectionString = connectionString;
    }

    public static DatabaseConfiguration FromAppSettings(string jsonFile)
    {
        DatabaseConfiguration configuration = new(jsonFile);
        return configuration;
    }
}
