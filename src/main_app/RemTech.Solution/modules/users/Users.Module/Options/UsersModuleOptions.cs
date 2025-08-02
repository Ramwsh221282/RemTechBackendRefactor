using Microsoft.Extensions.Configuration;

namespace Users.Module.Options;

public sealed record UsersModuleOptions
{
    internal DatabaseOptions Database { get; }

    public UsersModuleOptions(string jsonFilePath)
    {
        Database = FromJson(jsonFilePath);
    }

    private static DatabaseOptions FromJson(string jsonFilePath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonFilePath).Build();
        IConfigurationSection section = root.GetSection(nameof(UsersModuleOptions))
            .GetSection(nameof(DatabaseOptions));
        string? host = section.GetValue<string>("Host");
        string? port = section.GetValue<string>("Port");
        string? database = section.GetValue<string>("Database");
        string? userName = section.GetValue<string>("UserName");
        string? password = section.GetValue<string>("Password");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException($"{nameof(DatabaseOptions)} empty host database value.");
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException($"{nameof(DatabaseOptions)} empty port database value.");
        if (string.IsNullOrWhiteSpace(database))
            throw new ApplicationException($"{nameof(DatabaseOptions)} empty database name value.");
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(DatabaseOptions)} empty database user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(DatabaseOptions)} empty password database value."
            );
        return new DatabaseOptions(host, port, database, userName, password);
    }
}

internal sealed record DatabaseOptions(
    string Host,
    string Port,
    string Database,
    string UserName,
    string Password
)
{
    private const string ConnectionStringTemplate =
        "Host={0};Port={1};Database={2};Username={3};Password={4};";

    public string ToConnectionString()
    {
        return string.Format(ConnectionStringTemplate, Host, Port, Database, UserName, Password);
    }
}
