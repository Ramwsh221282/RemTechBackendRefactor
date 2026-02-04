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
            services.RegisterParserDependencies(isDevelopment);
            services.AddContainedItemsProducer();
            services.AddFinishParserProducer();
            services.RegisterAvitoFirewallBypass();
            services.RegisterParserSubscriptionProcess();
            services.RegisterParserWorkStages();
            services.RegisterTextTransformerBuilder();
        }

        public void RegisterInfrastructureDependencies(bool isDevelopment)
        {
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
