namespace Parsing.RabbitMq.Configuration;

public sealed class RabbitMqConfigurationSource : IRabbitMqConfigurationSource
{
    private readonly bool _isDevelopment;

    public RabbitMqConfigurationSource(bool isDevelopment)
    {
        _isDevelopment = isDevelopment;
    }

    public RabbitMqConfiguration Provide()
    {
        if (_isDevelopment)
        {
            Console.WriteLine("Development environment");
            return new JsonRabbitMqConfigurationSource("appsettings.json").Provide();
        }
        Console.WriteLine("Production environment");
        return new EnvRabbitMqConfigurationSource().Provide();
    }
}
