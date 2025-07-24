using Avito.Parsing.Vehicles.VehiclesParsing;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.Vehicles.Common.Json;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Logging.Adapter;
using RemTech.Logging.Library;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Theory]
    [InlineData("https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/kobelco-ASgBAgICAkRU5k3Qxg3KrD4?cd=1", "GEOS_ML_1.txt")]
    [InlineData("https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/ekskavator-pogruzchik/amir-ASgBAgICA0RU5k3Qxg2MpvYR1MYNvrA~?cd=1", "GEOS_ML_2.txt")]
    [InlineData("https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0?cd=1&f=ASgBAgICA0RU4E3cxg3s~F3gxg2azk0", "GEOS_ML_3.txt")]
    [InlineData("https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/ponsse-ASgBAgICAkRUsiyexw346j8?cd=1", "GEOS_ML_4.txt")]
    private async Task Invoke_Catalogue_Parser(string url, string textFile)
    {
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
                await new DefaultBrowserInstantiation(
                    new HeadlessBrowserInstantiationOptions(),
                    new BasicBrowserLoading()).Instantiation(),
                url);
        using CommunicationChannel channel = new CommunicationChannel("http://localhost:5051");
        DatabaseConfiguration configuration = new DatabaseConfiguration("appsettings.json");
        ICustomLogger logger = new ConsoleLogger();
        await using PgConnectionSource dbPgConnection = new(configuration);
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        await using ITextWrite write = new LoggingTextWrite(logger, new TextWrite(AppDomain.CurrentDomain.BaseDirectory)
            .WithDirectory("GEOS_ML")
            .WithTextFile(textFile));
        List<IParsedVehicle> results = [];
        foreach (IPage page in pagesSource.Iterate())
        {
            IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                page,
                write,
                logger,
                dbPgConnection,
                channel,
                url);
            await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
            {
                IParsedVehicle temp = vehicle;
                if (await new ValidatingParsedVehicle(temp).IsValid())
                    results.Add(temp);
            }
        }
    }

    [Theory]
    [InlineData("https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/ponsse-ASgBAgICAkRUsiyexw346j8?cd=1")]
    private async Task Parsed_Vehicle_Json_Success(string url)
    {
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new HeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()).Instantiation(),
            url);
        using CommunicationChannel channel = new CommunicationChannel("http://localhost:5051");
        DatabaseConfiguration configuration = new DatabaseConfiguration("appsettings.json");
        ICustomLogger logger = new ConsoleLogger();
        await using PgConnectionSource dbPgConnection = new(configuration);
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        foreach (IPage page in pagesSource.Iterate())
        {
            IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                page,
                new NoTextWrite(),
                logger,
                dbPgConnection,
                channel,
                url);
            await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
            {
                IParsedVehicle temp = vehicle;
                if (!await new ValidatingParsedVehicle(temp).IsValid())
                    continue;
                ParsedVehicleInfo info = new(
                    await temp.Identity(),
                    await temp.Kind(),
                    await temp.Brand(),
                    await temp.Model(),
                    await temp.Price(),
                    await temp.Characteristics(),
                    await temp.Photos(),
                    await temp.Geo(),
                    new ParsedVehicleParser("Test Parser", "Test Type", "Test Link")
                );
                string json = info.Json().Read();
            }
        }
    }
}