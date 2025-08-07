using Microsoft.Extensions.Configuration;

namespace Parsing.RabbitMq.Configuration;

public sealed class JsonRabbitMqConfigurationSource(string jsonFilePath)
    : IRabbitMqConfigurationSource
{
    public IRabbitMqConfiguration Provide()
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonFilePath).Build();
        string? hostName = root.GetSection(ConfigurationConstants.HostNameKey).Value;
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.HostNameKey} not found."
            );
        string? userName = root.GetSection(ConfigurationConstants.UserNameKey).Value;
        if (string.IsNullOrWhiteSpace(userName))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.UserNameKey} not found."
            );
        string? password = root.GetSection(ConfigurationConstants.PasswordKey).Value;
        if (string.IsNullOrWhiteSpace(password))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.PasswordKey} not found."
            );
        string? port = root.GetSection(ConfigurationConstants.PortKey).Value;
        if (string.IsNullOrWhiteSpace(port))
            throw new ApplicationException(
                $"Rabbit mq configuration. {ConfigurationConstants.PortKey} not found."
            );
        return new JsonRabbitMqConfiguration(hostName, userName, password, port);
    }
}
