using AvitoSparesParser.CatalogueParsing;
using AvitoSparesParser.CatalogueParsing.Extensions;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractPagedUrls;

public sealed class ResilientExtractPagedUrlsCommand(
    Serilog.ILogger logger,
    IExtractPagedUrlsCommand inner,
    int attemptsCount = 5
) : IExtractPagedUrlsCommand
{
    public async Task<AvitoCataloguePage[]> Extract(string initialUrl)
    {
        int currentAttempt = 0;
        while (currentAttempt < attemptsCount)
        {
            try
            {
                return await inner.Extract(initialUrl);
            }
            catch (Exception ex)
            {
                if (currentAttempt == attemptsCount)
                {
                    logger.Error(
                        ex,
                        "Failed to extract paged urls from initial url: {Url} after {Attempts} attempts.",
                        initialUrl,
                        attemptsCount
                    );
                    throw;
                }

                currentAttempt++;
                logger.Warning(
                    ex,
                    "Attempt {Attempt} to extract paged urls from initial url: {Url} failed. Retrying...",
                    currentAttempt,
                    initialUrl
                );
            }
        }

        // в while true уже есть выход.
        throw new InvalidOperationException("Unreachable code.");
    }
}

public sealed class ExtractPagedUrlsCommand(
    Func<Task<IPage>> pageSource,
    AvitoBypassFactory bypassFactory
) : IExtractPagedUrlsCommand
{
    public async Task<AvitoCataloguePage[]> Extract(string initialUrl)
    {
        IPage page = await pageSource();
        IAvitoBypassFirewall bypass = bypassFactory.Create(page);
        string[] pagedUrls = await new AvitoPagedUrlsExtractor(page, bypass).ExtractUrls(
            initialUrl
        );
        return [.. pagedUrls.Select(AvitoCataloguePage.New)];
    }
}
