using AvitoFirewallBypass;
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
            services.RegisterParserDependencies(isDevelopment);
            services.RegisterParserWorkStagesContext();
            services.RegisterParserSubscription();
            services.RegisterTextTransformerBuilder();
            services.RegisterAvitoFirewallBypass();
            services.AddFinishParserProducer();
            services.AddContainedItemsProducer();
        }
    }
}
