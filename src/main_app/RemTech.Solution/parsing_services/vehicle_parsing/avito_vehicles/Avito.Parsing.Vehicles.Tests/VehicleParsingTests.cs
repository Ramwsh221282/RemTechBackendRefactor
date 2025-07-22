using Avito.Parsing.Vehicles.PaginationBar;
using Avito.Parsing.Vehicles.VehiclesParsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.Logging;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Common.ParsedVehicles;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task Invoke_Catalogue_Parser()
    {
        string[] urls =
        [
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/liugong-ASgBAgICAkRU5k3Qxg3urD4?cd=1",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/hangcha-ASgBAgICAkRU4E3cxg26rj8?cd=1",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0?cd=1&f=ASgBAgICA0RU4E3cxg2Mrj_gxg2azk0",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/ace-ASgBAgICAkRU5k3Qxg3Mqj4?cd=1",
        ];

        foreach (string url in urls)
        {
            await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
                await new DefaultBrowserInstantiation(
                    new NonHeadlessBrowserInstantiationOptions(),
                    new BasicBrowserLoading()).Instantiation(),
                url);
            IParsingLog log = new ConsoleParsingLog();
            await using IBrowserPagesSource pagesSource = await browser.AccessPages();
            try
            {
                foreach (IPage page in pagesSource.Iterate())
                {
                    IPageAction bottomScrolling = new PageBottomScrollingAction(page);
                    IAvitoBypassFirewall bypass =
                        new AvitoBypassFirewallLogging(log,
                            new AvitoBypassWebsiteIsNotAvailable(page,
                                new AvitoBypassRepetetive(
                                    page,
                                    new AvitoBypassFirewallLazy(
                                        page,
                                        new AvitoBypassFirewallExceptionSupressing(
                                            new AvitoBypassFirewall(page))))));
                    IAvitoPaginationBarSource barSource =
                        new LoggingAvitoPaginationBarSource(log,
                            new BottomScrollingAvitoPaginationBarSource(page,
                                new AvitoPaginationBarSource(page)));
                    IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                        page,
                        log,
                        bypass,
                        barSource,
                        bottomScrolling,
                        url);
                    await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
                    {
                        IParsedVehicle temp = vehicle;
                    }
                }
            }
            catch(Exception ex)
            {
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrowsingCrashes");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                string fileName = $"{Guid.NewGuid()}.txt";
                string filePath = Path.Combine(directoryPath, fileName);
                await File.WriteAllTextAsync(filePath, ex.ToString());
            }
        }
    }
}