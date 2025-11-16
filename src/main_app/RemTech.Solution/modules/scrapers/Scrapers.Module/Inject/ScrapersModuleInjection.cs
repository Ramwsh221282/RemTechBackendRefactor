using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Scrapers.Module.Domain.JournalsContext.BackgroundServices.AddJournalRecordListener;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Features.CreateNewParser.Inject;
using Scrapers.Module.Features.FinishParser.Entrance;
using Scrapers.Module.Features.FinishParserLink.Entrance;
using Scrapers.Module.Features.IncreaseProcessedAmount.Entrance;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;
using Scrapers.Module.Features.StartParser.RabbitMq;
using Scrapers.Module.ParserStateCache;

namespace Scrapers.Module.Inject;

public static class ScrapersModuleInjection
{
    public static void InjectScrapersModule(this IServiceCollection services)
    {
        CreateNewParserInjection.Inject(services);
        services.AddHostedService<FinishParserEntrance>();
        services.AddHostedService<FinishedParserLinkEntrance>();
        services.AddHostedService<IncreasedProcessedEntrance>();
        services.AddHostedService<AddJournalRecordBackgroundService>();
        services.AddSingleton<IParserStartedPublisher, RabbitMqParserStartedPublisher>();
        services.AddSingleton(Channel.CreateUnbounded<IncreaseProcessedMessage>());
        services.AddSingleton<IIncreaseProcessedPublisher, IncreaseProcessedPublisher>();
        services.AddSingleton<ParserStateCachedStorage>();
        services.AddSingleton<ActiveScraperJournalsCache>();
    }
}
