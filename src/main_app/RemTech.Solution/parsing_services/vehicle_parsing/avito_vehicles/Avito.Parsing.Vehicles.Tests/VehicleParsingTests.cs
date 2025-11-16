using Avito.Vehicles.Service.VehiclesParsing;
using Parsing.RabbitMq.PublishVehicle.Extras;
using Parsing.SDK.Browsers;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;
using Serilog;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task Test()
    {
        string link =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/komatsu-ASgBAgICAkRUsiyexw3g6j8?cd=1";
        await using IScrapingBrowser browser = await BrowserFactory.ProvideBrowser();
        await using IPage page = await browser.ProvideDefaultPage();
        await new PageNavigating(page, link).Do();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
            page,
            new NoTextWrite(),
            logger,
            link,
            null! // should be replaced by DuplicateIdsGrpcClient
        );
        await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
        {
            if (!await new ValidatingParsedVehicle(vehicle).IsValid())
                continue;
            SentencesCollection sentences = await vehicle.Sentences();
            string description = sentences.FormText();
            if (string.IsNullOrWhiteSpace(description))
                logger.Warning("No description provided.");
        }
    }
}
