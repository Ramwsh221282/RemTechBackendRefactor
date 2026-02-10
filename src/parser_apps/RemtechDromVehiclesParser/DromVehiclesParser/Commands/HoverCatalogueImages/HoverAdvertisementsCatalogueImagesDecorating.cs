using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.HoverCatalogueImages;

public static class HoverAdvertisementsCatalogueImagesDecorating
{
    extension(IHoverAdvertisementsCatalogueImagesCommand command)
    {
        public IHoverAdvertisementsCatalogueImagesCommand UseLogging(Serilog.ILogger logger)
        {
            return new HoverAdvertisementsCatalogueImagesLogging(logger, command);
        }

        public IHoverAdvertisementsCatalogueImagesCommand UseResilience(BrowserManager manager, IPage page)
        {
            return new HoverAdvertisementsCatalogueImagesResilient(manager, page, command);
        }
    }
}