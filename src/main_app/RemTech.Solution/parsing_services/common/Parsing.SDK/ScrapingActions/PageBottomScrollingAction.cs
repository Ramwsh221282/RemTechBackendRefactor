using PuppeteerSharp;

namespace Parsing.SDK.ScrapingActions;

public sealed class PageBottomScrollingAction : IPageAction
{
    private readonly IPage _page;
    private const string CurrentHeightScript = "document.body.scrollHeight";
    private const string ScrollBottomScript = "window.scrollTo(0, document.body.scrollHeight)";

    public PageBottomScrollingAction(IPage page)
    {
        _page = page;
    }

    public async Task Do()
    {
        int currentHeight = await _page.EvaluateExpressionAsync<int>(CurrentHeightScript);
        while (true)
        {
            await _page.EvaluateExpressionAsync(ScrollBottomScript);
            await Task.Delay(TimeSpan.FromSeconds(1));
            int newHeight = await _page.EvaluateExpressionAsync<int>(CurrentHeightScript);
            if (newHeight == currentHeight)
                break;
            currentHeight = newHeight;
        }
    }
}
