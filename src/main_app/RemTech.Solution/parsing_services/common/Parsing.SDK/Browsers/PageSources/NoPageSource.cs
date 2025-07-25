using PuppeteerSharp;

namespace Parsing.SDK.Browsers.PageSources;

public sealed class NoPageSource : IBrowserPagesSource
{
    public void Dispose() =>
        throw new ApplicationException($"{nameof(NoPageSource)} contain no pages.");

    public ValueTask DisposeAsync() =>
        throw new ApplicationException($"{nameof(NoPageSource)} contain no pages.");

    public IEnumerable<IPage> Iterate() =>
        throw new ApplicationException($"{nameof(NoPageSource)} contain no pages.");

    public async Task<SinglePageSource> Single(IBrowser browser, string sourceUrl)
    {
        IPage page = await browser.NewPageAsync();
        try
        {
            NavigationOptions opts = new()
            {
                Timeout = 0,
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
            };
            await page.GoToAsync(sourceUrl, opts);
        }
        catch
        {
            Console.WriteLine("Exception as instantiating single page source with DOM Content load. Ignored.");
        }
        
        return new SinglePageSource(page, sourceUrl);
    }
}