using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.PrepareAvitoPage;

public static class PrepareAvitoPageCommandDecorating
{
    extension(IPrepareAvitoPageCommand command)
    {
        public IPrepareAvitoPageCommand UseLogging(Serilog.ILogger logger)
        {
            return new PrepareAvitoPageLogging(logger, command);
        }

        public IPrepareAvitoPageCommand UseResilience(BrowserManager manager, IPage page)
        {
            return new PrepareAvitoPageCommandResilient(manager, page, command);
        }
    }
}