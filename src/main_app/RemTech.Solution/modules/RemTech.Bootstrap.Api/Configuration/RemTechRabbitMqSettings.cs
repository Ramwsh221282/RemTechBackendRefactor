namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechRabbitMqSettings
{
    private const string HostKey = ConfigurationConstants.RABBIT_HOST_NAME_KEY;
    private const string PortKey = ConfigurationConstants.RABBIT_PORT_KEY;
    private const string UserKey = ConfigurationConstants.RABBIT_USER_NAME_KEY;
    private const string PasswordKey = ConfigurationConstants.RABBIT_PASSWORD_KEY;
    private const string Context = nameof(RemTechRabbitMqSettings);
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }
    public string Port { get; }

    private RemTechRabbitMqSettings(string hostName, string userName, string password, string port)
    {
        HostName = hostName;
        UserName = userName;
        Password = password;
        Port = port;
    }

    public static RemTechRabbitMqSettings CreateFromJson(string jsonPath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
        string? host = root.GetSection(HostKey).Value;
        string? port = root.GetSection(PortKey).Value;
        string? userName = root.GetSection(UserKey).Value;
        string? password = root.GetSection(PasswordKey).Value;
        return FromValues(host, port, userName, password);
    }

    public static RemTechRabbitMqSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(HostKey);
        string? port = Environment.GetEnvironmentVariable(PortKey);
        string? userName = Environment.GetEnvironmentVariable(UserKey);
        string? password = Environment.GetEnvironmentVariable(PasswordKey);
        return FromValues(host, port, userName, password);
    }

    private static RemTechRabbitMqSettings FromValues(
        string? host,
        string? port,
        string? userName,
        string? password
    )
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException($"{Context} empty host rabbitmq value.");
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException($"{Context} empty port rabbitmq value.");
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException($"{Context} empty rabbitmq user name value.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException($"{Context} empty password rabbitmq value.");
        return new RemTechRabbitMqSettings(host, userName, password, port);
    }
}
