using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoFirewallBypass;

public sealed class AvitoPagedUrlsExtractor
{
    private readonly IPage _page;
    private readonly IAvitoBypassFirewall _bypass;

    public AvitoPagedUrlsExtractor(IPage page, IAvitoBypassFirewall bypass)
    {
        _page = page;
        _bypass = bypass;
    }

    public async Task<string[]> ExtractUrls(string initialUrl)
    {
        int maxPage = await ExtractMaxPage(initialUrl);
        return CreateByMaxPage(maxPage, initialUrl);
    }
    
    private string[] CreateByMaxPage(int maxPage, string initialUrl)
    {
        List<string> pagedUrls = [];
        for (int i = 1; i <= maxPage; i++)
            pagedUrls.Add(initialUrl + $"&p={i}");
        return pagedUrls.ToArray();
    }
    
    private async Task<int> ExtractMaxPage(string initialUrl)
    {
        JsonData current = new() { MaxPage = 1, PagedUrls = [initialUrl] };
        string mutatingUrl = initialUrl;
        while (true)
        {
            await _page.PerformQuickNavigation(mutatingUrl, timeout: 2000);
            if (!await _bypass.Bypass())
                continue;

            await WaitForPagination(_page);
            JsonData paginationInfo = await ExtractPaginationUrls(_page);

            if (paginationInfo.MaxPage <= current.MaxPage)
                break;
            
            current = paginationInfo;
            mutatingUrl = paginationInfo.PagedUrls[^1];
        }

        return current.MaxPage;
    }

    private static Task WaitForPagination(IPage page) => 
        page.ResilientWaitForSelector("nav[aria-label='Пагинация']", attempts: 5);

    private static async Task<JsonData> ExtractPaginationUrls(IPage page)
    {
        return await page.EvaluateFunctionAsync<JsonData>(@"
                                   () => {
                                   const currentUrl = window.location.href;
                                   const paginationSelector = document.querySelector('nav[aria-label=""Пагинация""]');
                                   if (!paginationSelector) return [currentUrl];
                                   const paginationGroupSelector = paginationSelector.querySelector('ul[data-marker=""pagination-button""]');
                                   if (!paginationGroupSelector) return [currentUrl];
                                   const pageNumbersArray = Array.from(paginationGroupSelector.querySelectorAll('span[class=""styles-module-text-Z0vDE""]')).map(s => parseInt(s.innerText, 10));
                                   const maxPage = Math.max(...pageNumbersArray);
                                   const pagedUrls = [];
                                   for (let i = 1; i <= maxPage; i++) {
                                       const pagedUrl = currentUrl + '&p=' + i;
                                       pagedUrls.push(pagedUrl);
                                   }
                                   return { maxPage: maxPage, pagedUrls: pagedUrls };
                                   }
                                   ");
    }

    private sealed class JsonData
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}