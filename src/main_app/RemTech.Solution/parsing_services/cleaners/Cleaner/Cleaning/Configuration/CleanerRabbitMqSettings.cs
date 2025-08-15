using RabbitMQ.Client;

namespace Cleaner.Cleaning.Configuration;

internal sealed class CleanerRabbitMqSettings
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _port;

    private CleanerRabbitMqSettings(string hostName, string userName, string password, string port)
    {
        _hostName = hostName;
        _userName = userName;
        _password = password;
        _port = port;
    }

    public static CleanerRabbitMqSettings FromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(ConfigurationConstants.RABBIT_HOST_NAME_KEY).Value;
        string? port = root.GetSection(ConfigurationConstants.RABBIT_PORT_KEY).Value;
        string? userName = root.GetSection(ConfigurationConstants.RABBIT_USER_NAME_KEY).Value;
        string? password = root.GetSection(ConfigurationConstants.RABBIT_PASSWORD_KEY).Value;
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException(
                $"{nameof(CleanerRabbitMqSettings)} empty host rabbitmq value."
            );
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"{nameof(CleanerRabbitMqSettings)} empty port rabbitmq value."
            );
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"{nameof(CleanerRabbitMqSettings)} empty rabbitmq user name value."
            );
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"{nameof(CleanerRabbitMqSettings)} empty password rabbitmq value."
            );
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
