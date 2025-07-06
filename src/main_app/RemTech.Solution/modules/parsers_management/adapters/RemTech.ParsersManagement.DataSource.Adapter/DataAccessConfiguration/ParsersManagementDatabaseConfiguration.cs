using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;

public sealed class ParsersManagementDatabaseConfiguration
{
    public string ConnectionString { get; }

    public ParsersManagementDatabaseConfiguration(string jsonFilePath)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(jsonFilePath).Build();
        IConfigurationSection section = configuration
            .GetSection(nameof(ParsersManagementDatabaseConfiguration))
            .GetSection(nameof(ConnectionString));
        string? connectionString = section.Value;
        ArgumentNullException.ThrowIfNull(connectionString);
        ConnectionString = connectionString;
    }

    public static ParsersManagementDatabaseConfiguration FromAppSettings(string jsonFile)
    {
        ParsersManagementDatabaseConfiguration configuration = new(jsonFile);
        return configuration;
    }

    public static ParsersManagementDatabaseConfiguration FromAppSettings(
        IServiceCollection services,
        string jsonFile
    )
    {
        ParsersManagementDatabaseConfiguration configuration = new(jsonFile);
        services.AddSingleton(configuration);
        return configuration;
    }
}
