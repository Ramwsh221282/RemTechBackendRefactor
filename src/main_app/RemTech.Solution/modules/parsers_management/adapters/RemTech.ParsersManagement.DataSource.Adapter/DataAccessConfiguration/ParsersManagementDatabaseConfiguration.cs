using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

public sealed class ParsersManagementDatabaseConfiguration
{
    public string ConnectionString { get; }

    private ParsersManagementDatabaseConfiguration(string connectionString) =>
        ConnectionString = connectionString;

    public static ParsersManagementDatabaseConfiguration FromAppSettings(string jsonFile)
    {
        string connectionString = ReadConnectionString(jsonFile);
        ParsersManagementDatabaseConfiguration configuration = new(connectionString);
        return configuration;
    }

    public static ParsersManagementDatabaseConfiguration FromAppSettings(
        IServiceCollection services,
        string jsonFile
    )
    {
        string connectionString = ReadConnectionString(jsonFile);
        ParsersManagementDatabaseConfiguration configuration = new(connectionString);
        services.AddSingleton(configuration);
        return configuration;
    }

    public static string ReadConnectionString(string jsonFile)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(jsonFile).Build();
        IConfigurationSection section = configuration
            .GetSection(nameof(ParsersManagementDatabaseConfiguration))
            .GetSection(nameof(ConnectionString));
        string? connectionString = section.Value;
        ArgumentNullException.ThrowIfNull(connectionString);
        return connectionString;
    }
}
