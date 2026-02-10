using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractAdvertisementsFromCatalogue;

public static class ExtractAdvertisementsFromCatalogueDecorating
{
    extension(IExtractAdvertisementsFromCatalogueCommand command)
    {
        public IExtractAdvertisementsFromCatalogueCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractAdvertisementsFromCatalogueLogging(logger, command);
        }

        public IExtractAdvertisementsFromCatalogueCommand UseResilience(BrowserManager manager, IPage page)
        {
            return new ExtractAdvertisementsFromCatalogueResilient(manager, page, command);
        }
    }
}