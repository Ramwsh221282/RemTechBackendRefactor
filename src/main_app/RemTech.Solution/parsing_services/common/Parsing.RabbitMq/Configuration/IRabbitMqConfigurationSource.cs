namespace Parsing.RabbitMq.Configuration;

public interface IRabbitMqConfigurationSource
{
    IRabbitMqConfiguration Provide();
}
