namespace Parsing.RabbitMq.Configuration;

public sealed class EnvRabbitMqConfigurationSource : IRabbitMqConfigurationSource
{
    public RabbitMqConfiguration Provide()
    {
        string? hostName = Environment.GetEnvironmentVariable(ConfigurationConstants.HostNameKey);
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.HostNameKey} not found."
            );
        string? userName = Environment.GetEnvironmentVariable(ConfigurationConstants.UserNameKey);
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.UserNameKey} not found."
            );
        string? password = Environment.GetEnvironmentVariable(ConfigurationConstants.PasswordKey);
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.PasswordKey} not found."
            );
        string? port = Environment.GetEnvironmentVariable(ConfigurationConstants.PortKey);
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.PortKey} not found."
            );
        return new RabbitMqConfiguration(hostName, userName, password, port);
    }
}
