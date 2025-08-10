using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Parsing.Avito.Common.PaginationBar;

public sealed class BottomScrollingAvitoPaginationBarSource(
    IPage page,
    IAvitoPaginationBarSource origin
) : IAvitoPaginationBarSource
{
    public async Task<AvitoPaginationBarElement> Read()
    {
        await new PageBottomScrollingAction(page).Do();
        return await origin.Read();
    }
}
