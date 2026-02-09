using DromVehiclesParser.Parsing.ParsingStages.StageProcessStrategies;
using DromVehiclesParser.ResultsExporing.TextFileExporting;
using ParsingSDK;
using ParsingSDK.RabbitMq;
using ParsingSDK.TextProcessing;

namespace DromVehiclesParser.DependencyInjection;

public static class DependenciesForParsingInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterDependenciesForParsing(bool isDevelopment)
        {
            services.AddContainedItemsProducer();
            services.AddFinishParserProducer();
            services.RegisterTextTransformerBuilder();
            services.AddSingleton<IExporter<TextFile>, TextFileExporter>();
            services.AddSingleton<ParsingStageDependencies>();
            ParsingDependencyInjection.RegisterParserDependencies(services, isDevelopment);
        }
    }
}
