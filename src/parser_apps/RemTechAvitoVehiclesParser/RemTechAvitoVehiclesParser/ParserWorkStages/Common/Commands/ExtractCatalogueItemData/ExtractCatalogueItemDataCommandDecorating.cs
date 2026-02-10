using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractCatalogueItemData;

public static class ExtractCatalogueItemDataCommandDecorating
{
    extension(IExtractCatalogueItemDataCommand command)
    {
        public IExtractCatalogueItemDataCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractCatalogueItemDataLogging(logger, command);
        }

        public IExtractCatalogueItemDataCommand UseResilience(BrowserManager manager, IPage page, Serilog.ILogger logger)
        {
            return new ResilientExtractCatalogueItemDataCommand(
                page: page, 
                manager: manager, 
                logger: logger, 
                inner: command
                );
        }
    }
}