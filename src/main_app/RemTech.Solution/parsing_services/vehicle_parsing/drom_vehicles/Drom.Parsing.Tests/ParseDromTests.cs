using Drom.Parsing.Vehicles.Parsing.AttributeParsers;
using Drom.Parsing.Vehicles.Parsing.Grpcs;
using Drom.Parsing.Vehicles.Parsing.Models;
using Drom.Parsing.Vehicles.Parsing.Utilities;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using Serilog;

namespace Drom.Parsing.Tests;

public sealed class ParseDromTests
{
    [Fact]
    private async Task Invoke_Scraping()
    {
        CommunicationChannel grpcNer = new CommunicationChannel(
            new CommunicationChannelOptions("http://localhost:5051")
        );
        string url = "https://auto.drom.ru/spec/john-deere/forestry/all/";
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new HeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        CharacteristicsRecognitionFromText ctxRecognition = new(grpcNer);
        Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IBrowserPagesSource pages = await browser.AccessPages();
        foreach (IPage page in pages.Iterate())
        {
            DromPagesCursor cursor = new DromPagesCursor(page);
            while (true)
            {
                await cursor.Navigate();
                await new PageBottomScrollingAction(page).Do();
                await new PageUpperScrollingAction(page).Do();

                try
                {
                    await new DromImagesHoveringAction(page).Do();
                }
                catch (DromCatalogueNoItemsException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error("{Ex}", ex.Message);
                }
                DromCatalogueCarsCollection collection = new(page);
                IEnumerable<DromCatalogueCar> cars = await collection.Iterate();
                foreach (DromCatalogueCar item in cars)
                {
                    await item.Navigation().Navigate(page);
                    await new DromVehicleModelSource(page).Print(item);
                    await new DromVehicleBrandSource(page).Print(item);
                    await new DromVehicleKindSource(page).Print(item);
                    (await new DromVehicleCharacteristicSource(page).Read()).Print(item);
                    (await new CarPriceSource(page).Read()).Print(item);
                    (await new CarLocationSource(page).Read()).Print(item);
                    await new GrpcRecognizedCarCharacteristics(page, grpcNer).Print(item);
                    item.LogMessage().Log(logger);
                }
            }
        }
    }
}
