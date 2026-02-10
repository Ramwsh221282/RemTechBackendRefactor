using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractConcretePageItem;

public static class ExtractConcretePageItemCommandDecorating
{
    extension(IExtractConcretePageItemCommand command)
    {
        public IExtractConcretePageItemCommand UseLogging(Serilog.ILogger logger)
        {
            return new ExtractConcretePageItemLogging(logger, command);
        }

        public IExtractConcretePageItemCommand UseResilience(
            Serilog.ILogger logger,
            BrowserManager manager,
            IPage page,
            int attemptsCount = 5
        )
        {
            return new ResilientExtractConcretePageItemCommand(logger, manager, page, command, attemptsCount);
        }
    }
}
