using System.Text;
using System.Text.RegularExpressions;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.RabbitMq.PublishVehicle.Extras;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using Serilog;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task ExtractText()
    {
        Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        string url =
            "https://www.avito.ru/petrozavodsk/gruzoviki_i_spetstehnika/lesohozyaystvennyy_trelevochnyy_traktor_tlt-100_7303317342?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJPQWtCSTNrejdlY3JyaWxsIjt9kgJV6T8AAAA";

        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new HeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        foreach (var page in pagesSource.Iterate())
        {
            IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
                .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
                .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
                .WrapBy(p => new AvitoBypassRepetetive(page, p))
                .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
                .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
            await bypass.Read();
            IElementHandle element = await new PageElementSource(page).Read(
                "div[data-marker='item-view/item-description']"
            );
            string text = await new InnerTextFromWebElement(element).Read();
            SentencesCollection collection = new();
            collection.Fill(text);
            int a = 0;
        }
    }
}
