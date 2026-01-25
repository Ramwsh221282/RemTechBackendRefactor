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
            if (isDevelopment)
            {
                services.RegisterParserDependencies(options =>
                {
                    options.DevelopmentMode = true;
                    options.Headless = true;
                });
            }
            
            services.AddContainedItemsProducer();
            services.AddFinishParserProducer();
            services.RegisterTextTransformerBuilder();
            services.AddSingleton<IExporter<TextFile>, TextFileExporter>();
            services.AddSingleton<ParsingStageDependencies>();
        }
    }
}