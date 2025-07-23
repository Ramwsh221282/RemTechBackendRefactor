using Avito.Parsing.Vehicles.PaginationBar;
using Avito.Parsing.Vehicles.VehiclesParsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Logging.Adapter;
using RemTech.Logging.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task Invoke_Catalogue_Parser()
    {
        string[] urls =
        [
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/kobelco-ASgBAgICAkRU5k3Qxg3KrD4?cd=1",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/granit-ASgBAgICAkRU4E3cxg3Kwm4?cd=1",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/ekskavator-pogruzchik/amir-ASgBAgICA0RU5k3Qxg2MpvYR1MYNvrA~?cd=1",
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0?cd=1&f=ASgBAgICA0RU4E3cxg3s~F3gxg2azk0",
            // "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/komatsu-ASgBAgICAkRUsiyexw3g6j8?cd=1",
            // "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/ponsse-ASgBAgICAkRUsiyexw346j8?cd=1",
            // "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/john_deere-ASgBAgICAkRUsiyexw3W6j8?cd=1",
        ];

        foreach (string url in urls)
        {
            await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
                await new DefaultBrowserInstantiation(
                    new HeadlessBrowserInstantiationOptions(),
                    new BasicBrowserLoading()).Instantiation(),
                url);
            using CommunicationChannel channel = new CommunicationChannel("http://localhost:5051");
            await using ITextWrite write = new LoggingTextWrite(new TextWrite(AppDomain.CurrentDomain.BaseDirectory)
                .WithDirectory("ML_TEXTS")
                .WithTextFile("ML_TEXTS.txt"));
            DatabaseConfiguration configuration = new DatabaseConfiguration("appsettings.json");
            ICustomLogger logger = new ConsoleLogger();
            await using ConnectionSource dbConnection = new(configuration);
            await using IBrowserPagesSource pagesSource = await browser.AccessPages();
            try
            {
                foreach (IPage page in pagesSource.Iterate())
                {
                    IPageAction bottomScrolling = new PageBottomScrollingAction(page);
                    IAvitoBypassFirewall bypass =
                        new AvitoBypassFirewallLogging(logger,
                            new AvitoBypassWebsiteIsNotAvailable(page,
                                new AvitoBypassRepetetive(
                                    page,
                                    new AvitoBypassFirewallLazy(
                                        page,
                                        new AvitoBypassFirewallExceptionSupressing(
                                            new AvitoBypassFirewall(page))))));
                    IAvitoPaginationBarSource barSource =
                        new LoggingAvitoPaginationBarSource(logger,
                            new BottomScrollingAvitoPaginationBarSource(page,
                                new AvitoPaginationBarSource(page)));
                    IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                        page,
                        bypass,
                        barSource,
                        bottomScrolling,
                        write,
                        logger,
                        dbConnection,
                        channel,
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