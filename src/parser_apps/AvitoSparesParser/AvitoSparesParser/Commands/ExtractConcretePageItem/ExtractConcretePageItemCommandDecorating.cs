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
            int attemptsCount = 5
        )
        {
            return new ResilientExtractConcretePageItemCommand(logger, command, attemptsCount);
        }
    }
}
