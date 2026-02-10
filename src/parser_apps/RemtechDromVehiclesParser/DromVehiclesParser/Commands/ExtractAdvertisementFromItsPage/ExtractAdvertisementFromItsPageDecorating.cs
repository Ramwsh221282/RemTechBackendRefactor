using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractAdvertisementFromItsPage;

public static class ExtractAdvertisementFromItsPageDecorating
{
    extension(IExtractAdvertisementFromItsPageCommand command)
    {
        public IExtractAdvertisementFromItsPageCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractAdvertisementFromItsPageLogging(command, logger);
        }

        public IExtractAdvertisementFromItsPageCommand UseResilience(IPage page, BrowserManager manager)
        {
            return new ExtractAdvertisementFromItsPageResilient(manager, page, command);
        }
    }
}