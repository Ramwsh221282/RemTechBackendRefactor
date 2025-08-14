using System.Threading.Channels;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scrapers.Module.Features.CreateNewParser.Inject;
using Scrapers.Module.Features.FinishParser.Entrance;
using Scrapers.Module.Features.FinishParserLink.Entrance;
using Scrapers.Module.Features.IncreaseProcessedAmount.Entrance;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;
using Scrapers.Module.Features.StartParser.Entrance;
using Scrapers.Module.Features.StartParser.RabbitMq;
using Scrapers.Module.ParserStateCache;

namespace Scrapers.Module.Inject;

public static class ScrapersModuleInjection
{
    public static void InjectScrapersModule(this IServiceCollection services)
    {
        CreateNewParserInjection.Inject(services);
        services.InjectStartParserBackgroundJob();
        services.AddHostedService<FinishParserEntrance>();
        services.AddHostedService<FinishedParserLinkEntrance>();
        services.AddHostedService<IncreasedProcessedEntrance>();
        services.AddSingleton<IParserStartedPublisher, RabbitMqParserStartedPublisher>();
        services.AddSingleton(Channel.CreateUnbounded<IncreaseProcessedMessage>());
        services.AddSingleton<IIncreaseProcessedPublisher, IncreaseProcessedPublisher>();
        services.AddSingleton<ParserStateCachedStorage>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(ScrapersModuleInjection).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create scrapers database.");
    }

    private static void InjectStartParserBackgroundJob(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            JobKey jobKey = JobKey.Create(nameof(StartingParsersEntrance));
            options
                .AddJob<StartingParsersEntrance>(jobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(30).RepeatForever()
                        )
                );
        });
    }
}
