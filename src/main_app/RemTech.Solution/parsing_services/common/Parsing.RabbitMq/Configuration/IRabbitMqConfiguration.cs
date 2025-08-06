using Microsoft.Extensions.DependencyInjection;
using Parsing.RabbitMq.StartParsing;

namespace Parsing.RabbitMq.Configuration;

public interface IRabbitMqConfiguration
{
    public void Register(IServiceCollection services, StartParsingListenerOptions options);
}
