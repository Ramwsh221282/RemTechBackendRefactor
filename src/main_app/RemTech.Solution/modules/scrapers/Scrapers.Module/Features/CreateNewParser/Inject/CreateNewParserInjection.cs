using Microsoft.Extensions.DependencyInjection;
using Scrapers.Module.Features.CreateNewParser.RabbitMq;

namespace Scrapers.Module.Features.CreateNewParser.Inject;

public static class CreateNewParserInjection
{
    public static void Inject(IServiceCollection services)
    {
        services.AddHostedService<NewParsersEntrance>();
    }
}
