using Avito.Vehicles.Service.Parsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.Avito.Common.PaginationBar;
using Parsing.RabbitMq.PublishSpare;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using Serilog;

namespace Avito.Parsing.Spares.Tests;

public class ScrapingTests
{
    [Fact]
    private async Task Parse_Spares()
    {
        const string url = "https://www.avito.ru/all/zapchasti_i_aksessuary?q=ponsse";
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new NonHeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IBrowserPagesSource pages = await browser.AccessPages();
        foreach (IPage page in pages.Iterate())
        {
            IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
                .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
                .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
                .WrapBy(p => new AvitoBypassRepetetive(page, p))
                .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
                .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
            IAvitoPaginationBarSource pagination = new AvitoPaginationBarSource(page)
                .WrapBy(p => new BottomScrollingAvitoPaginationBarSource(page, p))
                .WrapBy(p => new LoggingAvitoPaginationBarSource(logger, p));
            if (!await bypass.Read())
                break;
            LoggingAvitoPaginationBarElement bar = new LoggingAvitoPaginationBarElement(
                logger,
                await pagination.Read()
            );
            SpareBodyValidator validator = new SpareBodyValidator();
            LinkedList<SpareBody> bodies = [];
            foreach (string pageUrl in bar.Iterate(url))
            {
                await new PageNavigating(page, pageUrl)
                    .WrapBy(n => new LoggingPageNavigating(logger, pageUrl, n))
                    .Do();
                IEnumerable<AvitoSpare> spares = await new BlockBypassingAvitoSparesCollection(
                    bypass,
                    new ImageHoveringAvitoSparesCollection(page, new AvitoSparesCollection(page))
                ).Read();
                foreach (AvitoSpare spare in spares)
                {
                    await spare.Navigate(page);
                    if (!await bypass.Read())
                        continue;
                    await new PageBottomScrollingAction(page).Do();
                    await new PageUpperScrollingAction(page).Do();
                    await new VariantDescriptionDetailsSource()
                        .With(new AvitoCharacteristicsDetailsSource(page))
                        .With(new AvitoDescriptionDetailsSource(page))
                        .Add(spare);
                    SpareBody body = spare.AsSpareBody();
                    if (!validator.IsValid(body))
                        continue;
                    bodies.AddFirst(body);
                }
            }
        }
    }
}
