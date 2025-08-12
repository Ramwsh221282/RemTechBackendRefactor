using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Brand;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Description;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Model;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Photos;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Price;
using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Url;
using Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.Avito.Common.PaginationBar;
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

namespace Avito.Vehicles.Service.VehiclesParsing;

public sealed class AvitoVehiclesParser(
    IPage page,
    ITextWrite write,
    Serilog.ILogger logger,
    string originUrl
) : IParsedVehicleSource
{
    private readonly IPageAction _bottomScroll = new PageBottomScrollingAction(page);

    public async IAsyncEnumerable<IParsedVehicle> Iterate()
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
            yield break;
        AvitoPaginationBarElement bar = await pagination.Read();
        foreach (string url in bar.Iterate(originUrl))
        {
            IPageNavigating catalogueNavigating = new PageNavigating(page, url).WrapBy(
                n => new LoggingPageNavigating(logger, url, n)
            );
            CatalogueItemsList items = await new EmptyAvitoCatalogueItemsSource()
                .WrapBy(s => new NavigatingCatalogueItems(catalogueNavigating, s))
                .WrapBy(s => new ImageHoveringAvitoCatalogueItemsSource(page, s))
                .WrapBy(s => new ExtractingAvitoCatalogueItemsSource(page, s))
                .WrapBy(s => new IdentifiedAvitoCatalogueItemsSource(s))
                .WrapBy(s => new LoggingAvitoCatalogueItemsSource(logger, s))
                .Read();
            foreach (CatalogueItem item in items.Iterate())
            {
                IParsedVehiclePriceSource priceSource = new AvitoPriceFromContainer(page)
                    .WrapBy(_ =>
                        new VarietAvitoVehiclePriceSource()
                            .With(new AvitoVehiclePriceSource(page))
                            .With(new AvitoPriceFromContainer(page))
                    )
                    .WrapBy(p => new DefaultVehiclePriceOnErrorSource(p))
                    .WrapBy(p => new LoggingVehiclePriceSource(logger, p));

                IParsedVehicleGeoSource geoSource = new VarietWebElementGeoSource()
                    .With(new DefaultOnErroVehicleGeo(new WebElementVehicleGeoSource(page, write)))
                    .WrapBy(g => new LoggingVehicleGeoSource(logger, g));

                IParsedVehicleModelSource modelSource = new VarietVehicleModelSource()
                    .With(new VehicleModelFromBreadcrumbsSource(page))
                    .With(new FromCharacteristicsModelSource(page))
                    .WrapBy(c => new LoggingModelSource(logger, c));

                IParsedVehicleKindSource kindSource = new DbOrParsedVehicleKindSource(page).WrapBy(
                    v => new LoggingKindSource(logger, v)
                );

                IParsedVehicleBrandSource brandSource = new VarietVehicleBrandSource()
                    .With(new VehicleBrandFromBreadcrumbsSource(page))
                    .With(new FromCharacteristicsBrandSource(page))
                    .WrapBy(b => new LoggingBrandSource(logger, b));

                IKeyValuedCharacteristicsSource ctxSource = new VariantVehicleCharacteristics()
                    .With(
                        new DefaultOnErrorCharacteristics(new KeyValuedAvitoCharacteristics(page))
                    )
                    .WrapBy(c => new LoggingCharacteristics(logger, c));

                IPageNavigating itemNavigating = new PageNavigating(page, item.ReadUrl()).WrapBy(
                    n => new LoggingPageNavigating(logger, item.ReadUrl(), n)
                );

                IParsedVehiclePhotos photos = new FromCatalogueItemPhotos(item)
                    .WrapBy(p => new DefaultOnErrorItemPhotos(p))
                    .WrapBy(p => new LoggingItemPhotos(logger, p));

                IParsedVehicleUrlSource source = new FromCatalogueUrl(item)
                    .WrapBy(u => new DefaultOnErrorVehicleUrl(u))
                    .WrapBy(u => new LoggingVehicleUrl(logger, u));

                IParsedVehicleIdentitySource identity = new FromCatalogueIdentity(item)
                    .WrapBy(i => new DefaultOnErrorVehicleIdentity(i))
                    .WrapBy(i => new LoggingVehicleIdentity(logger, i));

                IAvitoDescriptionSource descriptionSource = new AvitoDescriptionSource(page);

                yield return await new EmptyAvitoVehicle()
                    .WrapBy(v => new AvitoVehicleWithDescription(descriptionSource, v))
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
                    .WrapBy(v => new ExceptionLoggingVehicle(item.ReadUrl(), logger, v))
                    .VehicleSource();
            }
        }
    }
}
