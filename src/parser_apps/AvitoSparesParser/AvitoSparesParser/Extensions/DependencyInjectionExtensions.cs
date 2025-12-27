using AvitoSparesParser.Database;
using AvitoSparesParser.ParserStartConfiguration;
using AvitoSparesParser.ParserSubscription;
using AvitoSparesParser.ParsingStages;
using Microsoft.Extensions.Options;
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
                    conf.AddSingleton<IOptions<ScrapingBrowserOptions>>(_ => Options.Create(new ScrapingBrowserOptions
                    {
                        Headless = true,
                        DevelopmentMode = true,
                    }));
                });
                
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