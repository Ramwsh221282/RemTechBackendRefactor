using AvitoFirewallBypass;
using Microsoft.Extensions.Options;
using ParsingSDK;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;
using RemTechAvitoVehiclesParser.ParserWorkStages;

namespace RemTechAvitoVehiclesParser;

public static class ParserDependenciesInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterDependenciesForParsing(bool isDevelopment)
        {
            services.RegisterParserWorkStagesContext();
            services.RegisterParserSubscription();
            services.RegisterTextTransformerBuilder();
            services.RegisterAvitoFirewallBypass();
            services.AddFinishParserProducer();
            services.AddContainedItemProducer();
            
            if (isDevelopment)
            {
                services.RegisterParserDependencies(conf =>
                {
                    IOptions<ScrapingBrowserOptions> options = Options.Create(new ScrapingBrowserOptions()
                    {
                        DevelopmentMode = true,
                        Headless = false,
                    });
                    conf.AddSingleton(options);
                });
            }
        }
    }
}