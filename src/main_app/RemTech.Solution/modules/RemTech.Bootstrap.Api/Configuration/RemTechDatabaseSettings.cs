namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechDatabaseSettings
{
    private const string HostKey = ConfigurationConstants.PG_HOST_NAME_KEY;
    private const string PasswordKey = ConfigurationConstants.PG_PASSWORD_KEY;
    private const string PortKey = ConfigurationConstants.PG_PORT_KEY;
    private const string UserKey = ConfigurationConstants.PG_USER_NAME_KEY;
    private const string DatabaseKey = ConfigurationConstants.PG_DATABASE_NAME_KEY;
    private const string Context = nameof(RemTechDatabaseSettings);

    public string Host { get; }
    public string Port { get; }
    public string Database { get; }
    public string Username { get; }
    public string Password { get; }

    private RemTechDatabaseSettings(
        string host,
        string port,
        string database,
        string username,
        string password
    )
    {
        Host = host;
        Port = port;
        Database = database;
        Username = username;
        Password = password;
    }

    private const string ConnectionStringTemplate =
        "Host={0};Port={1};Database={2};Username={3};Password={4};";

    public string ToConnectionString()
    {
        return string.Format(ConnectionStringTemplate, Host, Port, Database, Username, Password);
    }

    public static RemTechDatabaseSettings CreateFromJson(string jsonPath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
        string? host = root.GetSection(HostKey).Value;
        string? port = root.GetSection(PortKey).Value;
        string? database = root.GetSection(DatabaseKey).Value;
        string? userName = root.GetSection(UserKey).Value;
        string? password = root.GetSection(PasswordKey).Value;
        return CreateFromValues(host, port, database, userName, password);
    }

    public static RemTechDatabaseSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(HostKey);
        string? port = Environment.GetEnvironmentVariable(PortKey);
        string? database = Environment.GetEnvironmentVariable(DatabaseKey);
        string? username = Environment.GetEnvironmentVariable(UserKey);
        string? password = Environment.GetEnvironmentVariable(PasswordKey);
        return CreateFromValues(host, port, database, username, password);
    }

    private static RemTechDatabaseSettings CreateFromValues(
        string? host,
        string? port,
        string? database,
        string? userName,
        string? password
    )
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException($"{Context} empty host database value.");
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException($"{Context} empty port database value.");
        if (string.IsNullOrWhiteSpace(database))
            throw new ApplicationException($"{Context} empty database name value.");
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException($"{Context} empty database user name value.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException($"{Context} empty password database value.");
        return new RemTechDatabaseSettings(host, port, database, userName, password);
    }
}
