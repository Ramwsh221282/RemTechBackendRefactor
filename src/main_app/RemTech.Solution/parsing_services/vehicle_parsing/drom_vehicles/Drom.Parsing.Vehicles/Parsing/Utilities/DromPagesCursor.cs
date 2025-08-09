using System.Text;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Utilities;

public sealed class DromPagesCursor(IPage page)
{
    private readonly string _initialPage = page.Url;
    private int _currentPageNumber = 1;

    public async Task Navigate()
    {
        if (_currentPageNumber == 1)
        {
            _currentPageNumber += 1;
            return;
        }
        StringBuilder sb = new StringBuilder(_initialPage);
        sb.Append($"page{_currentPageNumber}/");
        _currentPageNumber += 1;
        await new PageNavigating(page, sb.ToString()).Do();
        await Task.Delay(TimeSpan.FromSeconds(6));
    }
}
