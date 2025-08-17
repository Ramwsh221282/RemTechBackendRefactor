using RabbitMQ.Client;

namespace Cleaner.Configuration;

internal sealed class CleanerRabbitMqSettings
{
    private const string HostKey = ConfigurationConstants.RABBIT_HOST_NAME_KEY;
    private const string PortKey = ConfigurationConstants.RABBIT_PORT_KEY;
    private const string UserKey = ConfigurationConstants.RABBIT_USER_NAME_KEY;
    private const string PasswordKey = ConfigurationConstants.RABBIT_PASSWORD_KEY;
    private const string Context = nameof(CleanerRabbitMqSettings);
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _port;

    private CleanerRabbitMqSettings(string hostName, string userName, string password, string port)
    {
        Console.WriteLine(
            $"Rabbit host: {hostName} Rabbit user: {userName} Rabbit password: {password} Rabbit port: {port}"
        );
        _hostName = hostName;
        _userName = userName;
        _password = password;
        _port = port;
    }

    public static CleanerRabbitMqSettings FromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(HostKey).Value;
        string? port = root.GetSection(PortKey).Value;
        string? userName = root.GetSection(UserKey).Value;
        string? password = root.GetSection(PasswordKey).Value;
        return FromValues(host, userName, password, port);
    }

    public static CleanerRabbitMqSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(HostKey);
        string? port = Environment.GetEnvironmentVariable(PortKey);
        string? userName = Environment.GetEnvironmentVariable(UserKey);
        string? password = Environment.GetEnvironmentVariable(PasswordKey);
        return FromValues(host, userName, password, port);
    }

    private static CleanerRabbitMqSettings FromValues(
        string? host,
        string? userName,
        string? password,
        string? port
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
        return new CleanerRabbitMqSettings(host, userName, password, port);
    }

    public void Register(IServiceCollection services)
    {
        services.AddSingleton(
            new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                Port = int.Parse(_port),
            }
        );
    }
}
