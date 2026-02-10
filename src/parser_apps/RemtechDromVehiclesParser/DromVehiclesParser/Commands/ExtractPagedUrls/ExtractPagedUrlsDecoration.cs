using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractPagedUrls;

public static class ExtractPagedUrlsDecoration
{
    extension(IExtractPagedUrlsCommand command)
    {
        public IExtractPagedUrlsCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractPagedUrlsCommandLogging(logger, command);
        }

        public IExtractPagedUrlsCommand UseResilience(IPage page, BrowserManager manager)
        {
            return new ExtractPagedUrlsResilient(manager, page, command);
        }
    }
}