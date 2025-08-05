using Microsoft.Extensions.DependencyInjection;

namespace Parsing.RabbitMq;

public interface IRabbitMqConfiguration
{
    public void Register(IServiceCollection services);
}
