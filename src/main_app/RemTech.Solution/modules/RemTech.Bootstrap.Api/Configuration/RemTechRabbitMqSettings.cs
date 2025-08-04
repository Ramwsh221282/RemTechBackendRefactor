namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechRabbitMqSettings
{
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
        IConfigurationSection section = root.GetSection(nameof(RemTechApplicationSettings))
            .GetSection(nameof(RemTechRabbitMqSettings));
        string? host = section.GetValue<string>("Host");
        string? port = section.GetValue<string>("Port");
        string? userName = section.GetValue<string>("UserName");
        string? password = section.GetValue<string>("Password");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty host rabbitmq value."
            );
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty port rabbitmq value."
            );
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty rabbitmq user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(RemTechDatabaseSettings)} empty password rabbitmq value."
            );
        return new RemTechRabbitMqSettings(host, userName, password, port);
    }
}
