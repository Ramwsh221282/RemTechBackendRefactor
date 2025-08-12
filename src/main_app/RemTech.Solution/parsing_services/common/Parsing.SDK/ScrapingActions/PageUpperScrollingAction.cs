using PuppeteerSharp;

namespace Parsing.SDK.ScrapingActions;

public sealed class PageUpperScrollingAction : IPageAction
{
    private readonly IPage _page;
    private const string GetCurrentHeightScript = "document.body.scrollHeight";
    private const string ScrollScript = "window.scrollTo(0, 0)";

    public PageUpperScrollingAction(IPage page)
    {
        _page = page;
    }

    public async Task Do()
    {
        int currentHeight = await _page.EvaluateExpressionAsync<int>(GetCurrentHeightScript);
        while (true)
        {
            await _page.EvaluateExpressionAsync(ScrollScript);
            await Task.Delay(TimeSpan.FromSeconds(1));
            int newHeight = await _page.EvaluateExpressionAsync<int>(GetCurrentHeightScript);
            if (newHeight == currentHeight)
                break;
            currentHeight = newHeight;
        }
    }
}
