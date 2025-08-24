using Drom.Parsing.Vehicles.Parsing.AttributeParsers;
using Drom.Parsing.Vehicles.Parsing.Models;
using Drom.Parsing.Vehicles.Parsing.Utilities;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;
using Serilog;

namespace Drom.Parsing.Tests;

public sealed class ParseDromTests
{
    [Fact]
    private async Task Invoke_Scraping()
    {
        string url = "https://auto.drom.ru/spec/john-deere/forestry/all/";
        IScrapingBrowser browser = await BrowserFactory.ProvideDevelopmentBrowser();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IPage page = await browser.ProvideDefaultPage();
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
            IEnumerable<DromCatalogueCar> cars = await collection.Iterate(null!); // Null should be replaced by DuplicateIdsGrpcClient
            foreach (DromCatalogueCar item in cars)
            {
                await item.Navigation().Navigate(page);
                await new DromVehicleModelSource(page).Print(item);
                await new DromVehicleBrandSource(page).Print(item);
                await new DromVehicleKindSource(page).Print(item);
                (await new DromVehicleCharacteristicSource(page).Read()).Print(item);
                (await new CarPriceSource(page).Read()).Print(item);
                (await new CarLocationSource(page).Read()).Print(item);
                item.LogMessage().Log(logger);
            }
        }
    }
}
