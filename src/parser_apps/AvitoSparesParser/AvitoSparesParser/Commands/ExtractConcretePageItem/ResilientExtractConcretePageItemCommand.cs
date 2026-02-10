using AvitoSparesParser.AvitoSpareContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractConcretePageItem;

public sealed class ResilientExtractConcretePageItemCommand(
    Serilog.ILogger logger,
    BrowserManager manager,
    IPage browserPage,
    IExtractConcretePageItemCommand inner,
    int attemptsCount = 5)
    : IExtractConcretePageItemCommand
{
    public async Task<AvitoSpare> Extract(AvitoSpare spare)
    {
        int currentAttempt = 0;
        while (currentAttempt < attemptsCount)
        {
            try
            {
                return await inner.Extract(spare);
            }
            catch (Exception ex)
            {
                if (currentAttempt == attemptsCount)
                {
                    logger.Error(
                        ex,
                        "Failed to extract concrete page item from url: {Url} after {Attempts} attempts.",
                        spare.CatalogueRepresentation.Url,
                        attemptsCount
                    );
                    throw;
                }

                currentAttempt++;
                logger.Warning(
                    ex,
                    "Attempt {Attempt} to extract concrete page item from url: {Url} failed. Retrying...",
                    currentAttempt,
                    spare.CatalogueRepresentation.Url
                );
                
                browserPage = await manager.RecreatePage(browserPage);
                manager.ReleasePageUsedMemoryResources();
            }
        }

        // в while true уже есть выход.
        throw new InvalidOperationException("Unreachable code.");
    }
}