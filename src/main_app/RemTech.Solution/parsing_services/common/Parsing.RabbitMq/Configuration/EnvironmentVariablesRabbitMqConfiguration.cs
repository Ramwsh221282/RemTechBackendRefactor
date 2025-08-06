using Microsoft.Extensions.DependencyInjection;
using Parsing.RabbitMq.StartParsing;

namespace Parsing.RabbitMq.Configuration;

public sealed class EnvironmentVariablesRabbitMqConfiguration : IRabbitMqConfiguration
{
    public void Register(IServiceCollection services, StartParsingListenerOptions options)
    {
        throw new NotImplementedException();
    }
}
