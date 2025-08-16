namespace Parsing.RabbitMq.Configuration;

public interface IRabbitMqConfigurationSource
{
    RabbitMqConfiguration Provide();
}
