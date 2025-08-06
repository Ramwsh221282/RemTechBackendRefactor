using Microsoft.Extensions.DependencyInjection;

namespace Parsing.RabbitMq;

public static class CreateNewParserPublisherInjection
{
    public static void Inject(IServiceCollection services)
    {
        services.AddSingleton<ICreateNewParserPublisher, CreateNewParserPublisher>();
    }
}
