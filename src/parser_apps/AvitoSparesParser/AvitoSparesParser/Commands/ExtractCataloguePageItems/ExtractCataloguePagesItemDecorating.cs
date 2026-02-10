using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractCataloguePageItems;

public static class ExtractCataloguePagesItemDecorating
{
    extension(IExtractCataloguePagesItemCommand inner)
    {
        public IExtractCataloguePagesItemCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractCataloguePagesItemCommandLogging(logger, inner);
        }

        public IExtractCataloguePagesItemCommand UseResilience(
            Serilog.ILogger logger,
            BrowserManager manager,
            IPage page,
            int attemptsCount = 5
        )
        {
            return new ResilientCataloguePagesItemCommand(logger, manager, page, inner, attemptsCount);
        }
    }
}
