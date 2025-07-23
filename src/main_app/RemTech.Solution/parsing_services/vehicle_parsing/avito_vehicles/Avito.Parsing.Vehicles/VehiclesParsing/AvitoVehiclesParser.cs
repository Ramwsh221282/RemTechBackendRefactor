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
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing;

public sealed class AvitoVehiclesParser(
    IPage page,
    IAvitoBypassFirewall bypass, 
    IAvitoPaginationBarSource pagination,
    IPageAction bottomScroll,
    ITextWrite write,
    ICustomLogger logger,
    ConnectionSource connectionSource,
    CommunicationChannel channel,
    string originUrl) : IParsedVehicleSource
{
    public async IAsyncEnumerable<IParsedVehicle> Iterate()
    {
        if (!await bypass.Read())
            yield break;

        AvitoPaginationBarElement bar = await pagination.Read();
        foreach (string url in bar.Iterate(originUrl))
        {
            CatalogueItemsList catalogueItems = 
                await new LoggingAvitoCatalogueItemsSource(logger,
                    new IdentifiedAvitoCatalogueItemsSource(
                        new ExtractingAvitoCatalogueItemsSource(page,
                            new ImageHoveringAvitoCatalogueItemsSource(page,
                                    new NavigatingCatalogueItems(
                                        new LoggingPageNavigating(logger, url, new PageNavigating(page, url)), 
                                            new EmptyAvitoCatalogueItemsSource())))))
                    .Read();
            
            foreach (CatalogueItem item in catalogueItems.Iterate())
                yield return await new ExceptionLoggingVehicle(item.ReadUrl(), logger,
                                        new AvitoVehicleWithIdentity(
                                            new LoggingVehicleIdentity(logger,
                                                new DefaultOnErrorVehicleIdentity(
                                                    new FromCatalogueIdentity(item))),
                                        new AvitoVehicleWithSourceUrl(
                                            new LoggingVehicleUrl(logger, 
                                                new DefaultOnErrorVehicleUrl(
                                                    new FromCatalogueUrl(item))),
                                        new AvitoVehicleWithPhotos(
                                            new LoggingItemPhotos(logger,
                                                new DefaultOnErrorItemPhotos(new FromCatalogueItemPhotos(item))),
                                        new AvitoVehicleWithCharacteristics(
                                            new LoggingCharacteristics(logger,
                                             new VariantVehicleCharacteristics()
                                                .With(new DefaultOnErrorCharacteristics(new KeyValuedAvitoCharacteristics(page)))
                                                .With(new DefaultOnErrorCharacteristics(new GrpcRecognizedCharacteristics(page, channel)))
                                                .With(new DefaultOnErrorCharacteristics(new GrpcRecognizedCharacteristicsFromDescription(page, write, channel)))),
                                        new AvitoVehicleWithBrand(
                                            new LoggingBrandSource(logger, 
                                                new DbOrParsedVehicleBrandSource(connectionSource, logger, 
                                                    new DefaultOnErrorBrandSource(
                                                        new FromCharacteristicsBrandSource(page)))), 
                                            new AvitoVehicleWithKind(
                                                new LoggingKindSource(logger,
                                                    new DbOrParsedVehicleKindSource(connectionSource, logger, 
                                                        new DefaultOnErrorKindSource(
                                                            new VariantVehicleKind()
                                                                .With(new FromCharacteristicsKindSource(page))
                                                                .With(new DefaultOnErrorKindSource(new GrpcVehicleKindFromTitle(channel, write, page)))
                                                                .With(new DefaultOnErrorKindSource(new GrpcVehicleKindFromDescription(channel, page)))))),
                                            new AvitoVehicleWithModel(
                                                new LoggingModelSource(logger,
                                                    new DbOrParsedVehicleModel(connectionSource, logger, 
                                                        new DefaultOnErrorModel(
                                                            new FromCharacteristicsModelSource(page)))),
                                                new AvitoVehicleWithGeo(
                                                    new DbSearchedVehicleGeoSource(connectionSource, logger, 
                                                        new DefaultOnErroVehicleGeo(
                                                            new WebElementVehicleGeoSource(page))), 
                                                    new AvitoVehicleWithPrice(
                                                        new LoggingVehiclePriceSource(logger,
                                                            new DefaultVehiclePriceOnErrorSource(
                                                                new VarietAvitoVehiclePriceSource()
                                                                    .With(new AvitoVehiclePriceSource(page))
                                                                    .With(new AvitoPriceFromContainer(page)))),
                                                        new BottomScrollingAvitoVehicle(bottomScroll,
                                                            new BlockBypassingAvitoVehicle(bypass,
                                                                new NavigatingAvitoVehicle(
                                                                    new LoggingPageNavigating(logger, item.ReadUrl(),
                                                                        new PageNavigating(page, item.ReadUrl())),
                                                                    new EmptyAvitoVehicle())))
                                                    )))))))))).VehicleSource();
        }
    }
}