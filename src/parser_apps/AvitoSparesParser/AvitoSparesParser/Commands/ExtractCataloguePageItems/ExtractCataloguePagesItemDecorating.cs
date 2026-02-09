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
            int attemptsCount = 5
        )
        {
            return new ResilientCataloguePagesItemCommand(logger, inner, attemptsCount);
        }
    }
}
