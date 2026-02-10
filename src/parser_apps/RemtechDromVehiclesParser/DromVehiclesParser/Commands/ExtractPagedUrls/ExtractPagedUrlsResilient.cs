using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractPagedUrls;

public sealed class ExtractPagedUrlsResilient(
    BrowserManager manager, 
    IPage page, 
    IExtractPagedUrlsCommand inner, 
    int limit = 5)
    : IExtractPagedUrlsCommand
{
    public async Task<IEnumerable<DromCataloguePage>> Extract(string initialUrl)
    {
        int currentAttempt = 0;
        while (true)
        {
            if (currentAttempt > limit)
            {
                break;
            }

            try
            {
                return await inner.Extract(initialUrl);
            }
            catch(Exception ex) when (currentAttempt < limit)
            {
                page = await manager.RecreatePage(page);
                manager.ReleasePageUsedMemoryResources();
                currentAttempt++;
            }
        }
        
        throw new InvalidOperationException("Failed to extract paged urls");
    }
}