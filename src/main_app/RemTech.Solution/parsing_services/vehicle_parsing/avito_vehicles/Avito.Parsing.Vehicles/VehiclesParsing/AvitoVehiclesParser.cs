using Avito.Parsing.Vehicles.PaginationBar;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Photos;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Price;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;
using Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Core.Shared.Decorating;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing;

public sealed class AvitoVehiclesParser : IParsedVehicleSource
{
    private readonly IPage _page;
    private readonly ILogger _logger;
    private readonly string _originUrl;
    private readonly IPageAction _bottomScroll;
    private readonly CommunicationChannel _channel;
    private readonly ITextWrite _write;

    public AvitoVehiclesParser(
        IPage page,
        ITextWrite write,
        ILogger logger,
        CommunicationChannel channel,
        string originUrl
    )
    {
        _page = page;
        _logger = logger;
        _originUrl = originUrl;
        _channel = channel;
        _write = write;
        _bottomScroll = new PageBottomScrollingAction(page);
    }

    public async IAsyncEnumerable<IParsedVehicle> Iterate()
    {
        IAvitoBypassFirewall bypass = new AvitoBypassFirewall(_page)
            .WrapBy(p => new AvitoBypassFirewallExceptionSupressing(p))
            .WrapBy(p => new AvitoBypassFirewallLazy(_page, p))
            .WrapBy(p => new AvitoBypassRepetetive(_page, p))
            .WrapBy(p => new AvitoBypassWebsiteIsNotAvailable(_page, p))
            .WrapBy(p => new AvitoBypassFirewallLogging(_logger, p));
        IAvitoPaginationBarSource pagination = new AvitoPaginationBarSource(_page)
            .WrapBy(p => new BottomScrollingAvitoPaginationBarSource(_page, p))
            .WrapBy(p => new LoggingAvitoPaginationBarSource(_logger, p));
        if (!await bypass.Read())
            yield break;
        AvitoPaginationBarElement bar = await pagination.Read();
        foreach (string url in bar.Iterate(_originUrl))
        {
            IPageNavigating catalogueNavigating = new PageNavigating(_page, url).WrapBy(
                n => new LoggingPageNavigating(_logger, url, n)
            );
            CatalogueItemsList items = await new EmptyAvitoCatalogueItemsSource()
                .WrapBy(s => new NavigatingCatalogueItems(catalogueNavigating, s))
                .WrapBy(s => new ImageHoveringAvitoCatalogueItemsSource(_page, s))
                .WrapBy(s => new ExtractingAvitoCatalogueItemsSource(_page, s))
                .WrapBy(s => new IdentifiedAvitoCatalogueItemsSource(s))
                .WrapBy(s => new LoggingAvitoCatalogueItemsSource(_logger, s))
                .Read();
            foreach (CatalogueItem item in items.Iterate())
            {
                IParsedVehiclePriceSource priceSource = new AvitoPriceFromContainer(_page)
                    .WrapBy(_ =>
                        new VarietAvitoVehiclePriceSource()
                            .With(new AvitoVehiclePriceSource(_page))
                            .With(new AvitoPriceFromContainer(_page))
                    )
                    .WrapBy(p => new DefaultVehiclePriceOnErrorSource(p))
                    .WrapBy(p => new LoggingVehiclePriceSource(_logger, p));

                IParsedVehicleGeoSource geoSource = new VarietWebElementGeoSource()
                    .With(new DefaultOnErroVehicleGeo(new GrpcVehicleGeoSource(_page, _channel)))
                    .With(
                        new DefaultOnErroVehicleGeo(new WebElementVehicleGeoSource(_page, _write))
                    )
                    .WrapBy(g => new LoggingVehicleGeoSource(_logger, g));

                IParsedVehicleModelSource modelSource = new VarietVehicleModelSource()
                    .With(new VehicleModelFromBreadcrumbsSource(_page))
                    .With(new FromCharacteristicsModelSource(_page))
                    .WrapBy(c => new LoggingModelSource(_logger, c));

                IParsedVehicleKindSource kindSource = new DbOrParsedVehicleKindSource(
                    _page,
                    _channel
                ).WrapBy(v => new LoggingKindSource(_logger, v));

                IParsedVehicleBrandSource brandSource = new VarietVehicleBrandSource()
                    .With(new VehicleBrandFromBreadcrumbsSource(_page))
                    .With(new FromCharacteristicsBrandSource(_page))
                    .WrapBy(b => new LoggingBrandSource(_logger, b));

                IKeyValuedCharacteristicsSource ctxSource = new VariantVehicleCharacteristics()
                    .With(
                        new DefaultOnErrorCharacteristics(new KeyValuedAvitoCharacteristics(_page))
                    )
                    .With(
                        new DefaultOnErrorCharacteristics(
                            new GrpcRecognizedCharacteristics(_page, _channel)
                        )
                    )
                    .With(
                        new DefaultOnErrorCharacteristics(
                            new GrpcRecognizedCharacteristicsFromDescription(_page, _channel)
                        )
                    )
                    .WrapBy(c => new LoggingCharacteristics(_logger, c));

                IPageNavigating itemNavigating = new PageNavigating(_page, item.ReadUrl()).WrapBy(
                    n => new LoggingPageNavigating(_logger, item.ReadUrl(), n)
                );

                IParsedVehiclePhotos photos = new FromCatalogueItemPhotos(item)
                    .WrapBy(p => new DefaultOnErrorItemPhotos(p))
                    .WrapBy(p => new LoggingItemPhotos(_logger, p));

                IParsedVehicleUrlSource source = new FromCatalogueUrl(item)
                    .WrapBy(u => new DefaultOnErrorVehicleUrl(u))
                    .WrapBy(u => new LoggingVehicleUrl(_logger, u));

                IParsedVehicleIdentitySource identity = new FromCatalogueIdentity(item)
                    .WrapBy(i => new DefaultOnErrorVehicleIdentity(i))
                    .WrapBy(i => new LoggingVehicleIdentity(_logger, i));

                yield return await new EmptyAvitoVehicle()
                    .WrapBy(v => new AvitoVehicleWithPrice(priceSource, v))
                    .WrapBy(v => new AvitoVehicleWithGeo(geoSource, v))
                    .WrapBy(v => new AvitoVehicleWithModel(modelSource, v))
                    .WrapBy(v => new AvitoVehicleWithKind(kindSource, v))
                    .WrapBy(v => new AvitoVehicleWithBrand(brandSource, v))
                    .WrapBy(v => new AvitoVehicleWithCharacteristics(ctxSource, v))
                    .WrapBy(v => new AvitoVehicleWithPhotos(photos, v))
                    .WrapBy(v => new AvitoVehicleWithSourceUrl(source, v))
                    .WrapBy(v => new AvitoVehicleWithIdentity(identity, v))
                    .WrapBy(v => new BottomScrollingAvitoVehicle(_bottomScroll, v))
                    .WrapBy(v => new BlockBypassingAvitoVehicle(bypass, v))
                    .WrapBy(v => new NavigatingAvitoVehicle(itemNavigating, v))
                    .WrapBy(v => new ExceptionLoggingVehicle(item.ReadUrl(), _logger, v))
                    .VehicleSource();
            }
        }
    }
}
