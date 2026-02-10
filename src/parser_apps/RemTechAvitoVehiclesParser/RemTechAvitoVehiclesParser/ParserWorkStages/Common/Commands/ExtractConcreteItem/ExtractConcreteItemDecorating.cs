using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;

public static class ExtractConcreteItemDecorating
{
    extension(IExtractConcreteItemCommand command)
    {
        public IExtractConcreteItemCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractConcreteItemLogging(logger, command);
        }

        public IExtractConcreteItemCommand UseResilience(BrowserManager manager, IPage page)
        {
            return new ResilientExtractConcreteItemCommand(page, manager, command);
        }
    }
}