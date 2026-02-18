using DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;
using DromVehiclesParser.RabbitMq.Consumers;
using DromVehiclesParser.ResultsExporing.TextFileExporting;
using ParsingSDK;
using ParsingSDK.ParserStopingContext;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace DromVehiclesParser.DependencyInjection;

public static class DependenciesForParsingInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterDependenciesForParsing(bool isDevelopment)
        {
            services.AddScoped<IParserStopper, DromParserStopper>();
            services.AddContainedItemsProducer();
            services.AddFinishParserProducer();
            services.RegisterTextTransformerBuilder();
            services.AddSingleton<IExporter<TextFile>, TextFileExporter>();
            services.AddSingleton<ParsingStageDependencies>();
            ParsingDependencyInjection.RegisterParserDependencies(services, isDevelopment);
            services.AddTransient<IConsumer, ParserStopConsumer>();
            services.AddTransient<IConsumer, StartParserConsumer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }
    }
}
