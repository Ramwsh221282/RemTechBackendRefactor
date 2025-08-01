using Avito.Parsing.Vehicles.VehiclesParsing;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.Browsers;
using Parsing.SDK.Browsers.BrowserLoadings;
using Parsing.SDK.Browsers.InstantiationModes;
using Parsing.SDK.Browsers.InstantiationOptions;
using Parsing.SDK.Browsers.PageSources;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.RabbitMq.Adapter;
using Serilog;

namespace Avito.Parsing.Vehicles.Tests;

public class VehicleParsingTests
{
    [Fact]
    private async Task Bypass_Avito_Test()
    {
        string url =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/kobelco-ASgBAgICAkRU5k3Qxg3KrD4?cd=1";
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new NonHeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        ILogger logger = new LoggerSource().Logger();
        foreach (IPage page in pagesSource.Iterate())
        {
            IAvitoBypassFirewall bypass = new AvitoBypassFirewall(page)
                .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
                .WrapBy(p => new AvitoBypassFirewallLazy(page, p))
                .WrapBy(p => new AvitoBypassRepetetive(page, p))
                .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(page, p))
                .WrapBy(p => new AvitoBypassFirewallLogging(logger, p));
            Assert.True(await bypass.Read());
        }
    }

    [Theory]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/kobelco-ASgBAgICAkRU5k3Qxg3KrD4?cd=1",
        "GEOS_ML_1.txt"
    )]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/ekskavatory/ekskavator-pogruzchik/amir-ASgBAgICA0RU5k3Qxg2MpvYR1MYNvrA~?cd=1",
        "GEOS_ML_2.txt"
    )]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0?cd=1&f=ASgBAgICA0RU4E3cxg3s~F3gxg2azk0",
        "GEOS_ML_3.txt"
    )]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/ponsse-ASgBAgICAkRUsiyexw346j8?cd=1",
        "GEOS_ML_4.txt"
    )]
    private async Task Invoke_Catalogue_Parser(string url, string textFile)
    {
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new HeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        using CommunicationChannel channel = new CommunicationChannel("http://localhost:5051");
        DatabaseConfiguration configuration = new DatabaseConfiguration("appsettings.json");
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource dbPgConnection = new(configuration);
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        await using ITextWrite write = new LoggingTextWrite(
            logger,
            new TextWrite(AppDomain.CurrentDomain.BaseDirectory)
                .WithDirectory("GEOS_ML")
                .WithTextFile(textFile)
        );
        List<IParsedVehicle> results = [];
        foreach (IPage page in pagesSource.Iterate())
        {
            IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                page,
                write,
                logger,
                dbPgConnection,
                channel,
                url
            );
            await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
            {
                IParsedVehicle temp = vehicle;
                if (await new ValidatingParsedVehicle(temp).IsValid())
                    results.Add(temp);
            }
        }
    }

    [Theory]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/john_deere-ASgBAgICAkRUsiyexw3W6j8?cd=1"
    )]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/komatsu-ASgBAgICAkRUsiyexw3g6j8?cd=1"
    )]
    [InlineData(
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/lovol-ASgBAgICAkRUsiyexw2wlbAV?cd=1"
    )]
    private async Task Parsed_Vehicle_Json_Success(string url)
    {
        await using IScrapingBrowser browser = new SinglePagedScrapingBrowser(
            await new DefaultBrowserInstantiation(
                new HeadlessBrowserInstantiationOptions(),
                new BasicBrowserLoading()
            ).Instantiation(),
            url
        );
        using CommunicationChannel channel = new CommunicationChannel("http://localhost:5051");
        DatabaseConfiguration configuration = new DatabaseConfiguration("appsettings.json");
        RabbitMqConnectionOptions rabbitOptions = new("appsettings.json");
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource dbPgConnection = new(configuration);
        await using IBrowserPagesSource pagesSource = await browser.AccessPages();
        await using RabbitMqChannel rabbitChannel = new(rabbitOptions);
        await using RabbitSendPoint rabbitSendPoint = await rabbitChannel.MakeSendPoint(
            "vehicles_sink"
        );
        foreach (IPage page in pagesSource.Iterate())
        {
            IParsedVehicleSource vehicleSource = new AvitoVehiclesParser(
                page,
                new NoTextWrite(),
                logger,
                dbPgConnection,
                channel,
                url
            );
            await foreach (IParsedVehicle vehicle in vehicleSource.Iterate())
            {
                await new RabbitPublishingParsedVehicle(
                    logger,
                    vehicle,
                    rabbitSendPoint
                ).SendAsync();
            }
        }
    }
}
