using AvitoSparesParser.Database;
using AvitoSparesParser.ParserSubscription;
using AvitoSparesParser.ParsingStages;
using Microsoft.Extensions.Options;
using ParsingSDK.RabbitMq;
using Quartz;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace AvitoSparesParser.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void RegisterDependenciesForParsing(bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.RegisterParserDependencies(conf =>
                {
                    conf.AddSingleton(_ =>
                        Options.Create(
                            new ScrapingBrowserOptions { Headless = false, DevelopmentMode = true }
                        )
                    );
                });

                services.AddContainedItemsProducer();
                services.AddFinishParserProducer();
                services.RegisterAvitoFirewallBypass();
                services.RegisterParserSubscriptionProcess();
                services.RegisterParserWorkStages();
                services.RegisterTextTransformerBuilder();
            }
        }

        public void RegisterInfrastructureDependencies(bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
            }

            services.AddPostgres();
            services.AddMigrations([typeof(SchemaMigrations).Assembly]);
            services.AddRabbitMq();
            services.AddCronScheduledJobs();
            services.AddQuartzHostedService(c =>
            {
                c.StartDelay = TimeSpan.FromSeconds(10);
                c.WaitForJobsToComplete = true;
            });
        }
    }
}
