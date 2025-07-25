using PuppeteerSharp;

namespace Parsing.SDK.ScrapingActions;

public sealed class PageUpperScrollingAction : IPageAction
{
    private readonly IPage _page;
    private readonly string _getCurrentHeightScript;
    private readonly string _scrollBottomScript;

    public PageUpperScrollingAction(IPage page)
    {
        _page = page;
        _getCurrentHeightScript = string.Intern("document.body.scrollHeight");
        _scrollBottomScript = string.Intern("window.scrollTo(0, 0)");
    }
    
    public async  Task Do()
    {
        int currentHeight = await _page.EvaluateExpressionAsync<int>(_getCurrentHeightScript);
        while (true)
        {
            await _page.EvaluateExpressionAsync(_scrollBottomScript);
            await Task.Delay(TimeSpan.FromSeconds(1));
            int newHeight = await _page.EvaluateExpressionAsync<int>(_getCurrentHeightScript);
            if (newHeight == currentHeight)
                break;
            currentHeight = newHeight;
        }
    }
}