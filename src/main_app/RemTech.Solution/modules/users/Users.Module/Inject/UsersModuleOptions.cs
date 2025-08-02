using Microsoft.Extensions.Configuration;

namespace Users.Module.Inject;

public sealed record UsersModuleOptions
{
    internal UsersModuleDatabaseConfiguration Database { get; }

    public UsersModuleOptions(string appSettingsJsonPath)
    {
        Database = InitializeDatabaseConfig(appSettingsJsonPath);
    }

    private static UsersModuleDatabaseConfiguration InitializeDatabaseConfig(string jsonFilePath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonFilePath).Build();
        IConfigurationSection section = root.GetSection(nameof(UsersModuleOptions))
            .GetSection(nameof(UsersModuleDatabaseConfiguration));
        string? host = section.GetValue<string>("Host");
        string? port = section.GetValue<string>("Port");
        string? database = section.GetValue<string>("Database");
        string? userName = section.GetValue<string>("UserName");
        string? password = section.GetValue<string>("Password");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException(
                $"{nameof(UsersModuleDatabaseConfiguration)} empty host database value."
            );
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"{nameof(UsersModuleDatabaseConfiguration)} empty port database value."
            );
        if (string.IsNullOrWhiteSpace(database))
            throw new ApplicationException(
                $"{nameof(UsersModuleDatabaseConfiguration)} empty database name value."
            );
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(UsersModuleDatabaseConfiguration)} empty database user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(UsersModuleDatabaseConfiguration)} empty password database value."
            );
        return new UsersModuleDatabaseConfiguration(host, port, database, userName, password);
    }
}
