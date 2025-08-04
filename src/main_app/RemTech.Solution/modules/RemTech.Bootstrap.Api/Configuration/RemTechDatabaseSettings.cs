namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechDatabaseSettings
{
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
        IConfigurationSection section = root.GetSection(nameof(RemTechApplicationSettings))
            .GetSection(nameof(RemTechDatabaseSettings));
        string? host = section.GetValue<string>("Host");
        string? port = section.GetValue<string>("Port");
        string? database = section.GetValue<string>("Database");
        string? userName = section.GetValue<string>("UserName");
        string? password = section.GetValue<string>("Password");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty host database value."
            );
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty port database value."
            );
        if (string.IsNullOrWhiteSpace(database))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty database name value."
            );
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty database user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty password database value."
            );
        return new RemTechDatabaseSettings(host, port, database, userName, password);
    }
}
