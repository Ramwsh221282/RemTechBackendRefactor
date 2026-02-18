using AvitoFirewallBypass;
using ParsingSDK;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTechAvitoVehiclesParser.ParserWorkStages;
using RemTechAvitoVehiclesParser.RabbitMq.Consumers;

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
            services.AddTransient<IConsumer, ParserStartConsumer>();
            services.AddTransient<IConsumer, ParserStopConsumer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }
    }
}
