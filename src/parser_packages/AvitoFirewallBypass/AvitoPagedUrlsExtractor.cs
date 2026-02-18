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
        await _page.PerformNavigation(initialUrl);
        if (!await _bypass.Bypass())
        {
            throw new InvalidOperationException("Page is blocked.");
        }                

        PaginationResult result = await ExtractPaginationUsingJavaScript(_page);
        string[] pages = [.. result.PagedUrls.Select(u => u)];                        
        return pages;
    }


    private static async Task<PaginationResult> ExtractPaginationUsingJavaScript(IPage page)
    {
        await page.ScrollBottom();
        const string javaScript = """
            (() => {

            let maxPage = 0;
            const pagedUrls = [];

            const currentUrl = window.location.href;
            const paginationSelector = document.querySelector('nav[aria-label="Пагинация"]');
            if (!paginationSelector) return { maxPage, pagedUrls };

            const paginationGroupSelector = paginationSelector.querySelector('ul[data-marker="pagination-button"]');
            if (!paginationGroupSelector) return { maxPage, pagedUrls };

            const pageNumbersArray = Array.from(
            paginationGroupSelector.querySelectorAll('span[class="styles-module-text-Z0vDE"]'))
                .map(s => parseInt(s.innerText, 10));

            maxPage = Math.max(...pageNumbersArray);

            for (let i = 1; i <= maxPage; i++) {
                let url = new URL(currentUrl);
                url.searchParams.set('p', i);
                pagedUrls.push(url.toString());
            }

            return { maxPage, pagedUrls };
            })();
            """;

        return await page.EvaluateExpressionAsync<PaginationResult>(javaScript);
    }

    private sealed class PaginationResult
    {
        public int MaxPage { get; set; }
        public string[] PagedUrls { get; set; } = [];
    }
}