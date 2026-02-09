using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.CatalogueParsing;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractCataloguePageItems;

public sealed class ResilientCataloguePagesItemCommand(
    Serilog.ILogger logger,
    BrowserManager manager,
    IPage browserPage,
    IExtractCataloguePagesItemCommand inner,
    int attemptsCount = 5)
    : IExtractCataloguePagesItemCommand
{
    public async Task<AvitoSpare[]> Extract(AvitoCataloguePage page)
    {
        int currentAttempt = 0;
        while (currentAttempt < attemptsCount)
        {
            try
            {
                return await inner.Extract(page);
            }
            catch (Exception ex)
            {
                if (currentAttempt == attemptsCount)
                {
                    logger.Error(
                        ex,
                        "Failed to extract catalogue page items from url: {Url} after {Attempts} attempts.",
                        page.Url,
                        attemptsCount
                    );
                    throw;
                }

                currentAttempt++;
                logger.Warning(
                    ex,
                    "Attempt {Attempt} to extract catalogue page items from url: {Url} failed. Retrying...",
                    currentAttempt,
                    page.Url
                );
                
                browserPage = await manager.RecreatePage(browserPage);
                manager.ReleasePageUsedMemoryResources();
            }
        }

        // в while true уже есть выход.
        throw new InvalidOperationException("Unreachable code.");
    }
}