namespace Parsing.RabbitMq;

public interface IRabbitMqConfigurationSource
{
    IRabbitMqConfiguration Provide();
}
