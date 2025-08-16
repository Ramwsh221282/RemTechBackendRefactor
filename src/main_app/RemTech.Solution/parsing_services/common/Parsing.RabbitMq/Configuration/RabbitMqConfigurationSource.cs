namespace Parsing.RabbitMq.Configuration;

public sealed class RabbitMqConfigurationSource : IRabbitMqConfigurationSource
{
    public RabbitMqConfiguration Provide()
    {
        try
        {
            return new JsonRabbitMqConfigurationSource("appsettings.json").Provide();
        }
        catch
        {
            return new EnvRabbitMqConfigurationSource().Provide();
        }
    }
}
