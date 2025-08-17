using Avito.Vehicles.Service.VehiclesParsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.RabbitMq.PublishVehicle;
using Parsing.RabbitMq.PublishVehicle.Extras;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingActions;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using Serilog;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task Test()
    {
        string link =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/komatsu-ASgBAgICAkRUsiyexw3g6j8?cd=1";
        await using IScrapingBrowser browser = await BrowserFactory.ProvideDevelopmentBrowser();
        await using IPage page = await browser.ProvideDefaultPage();
        await new PageNavigating(page, link).Do();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
            page,
            new NoTextWrite(),
            logger,
            link
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
