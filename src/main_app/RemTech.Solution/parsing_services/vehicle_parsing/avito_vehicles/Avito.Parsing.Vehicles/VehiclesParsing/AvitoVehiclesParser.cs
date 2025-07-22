using Avito.Parsing.Vehicles.PaginationBar;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Photos;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Price;
using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;
using Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;
using Parsing.Avito.Common.BypassFirewall;
using Parsing.SDK.Logging;
using Parsing.SDK.ScrapingActions;
using Parsing.Vehicles.Common.ParsedVehicles;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing;

public sealed class AvitoVehiclesParser(
    IPage page,
    IParsingLog log,
    IAvitoBypassFirewall bypass, 
    IAvitoPaginationBarSource pagination,
    IPageAction bottomScroll,
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
                await new LoggingAvitoCatalogueItemsSource(log,
                    new IdentifiedAvitoCatalogueItemsSource(
                        new ExtractingAvitoCatalogueItemsSource(page,
                            new ImageHoveringAvitoCatalogueItemsSource(page,
                                    new NavigatingCatalogueItems(
                                        new LoggingPageNavigating(log, url, new PageNavigating(page, url)), 
                                            new EmptyAvitoCatalogueItemsSource())))))
                    .Read();
            
            foreach (CatalogueItem item in catalogueItems.Iterate())
                yield return await new ExceptionLoggingVehicle(item.ReadUrl(), log,
                    new AvitoVehicleWithIdentity(
                            new LoggingVehicleIdentity(log,
                                new DefaultOnErrorVehicleIdentity(
                                    new FromCatalogueIdentity(item))),
                        new AvitoVehicleWithSourceUrl(
                            new LoggingVehicleUrl(log, new DefaultOnErrorVehicleUrl(new FromCatalogueUrl(item))),
                            new AvitoVehicleWithPhotos(
                                new LoggingItemPhotos(log,
                                    new DefaultOnErrorItemPhotos(new FromCatalogueItemPhotos(item))),
                                new AvitoVehicleWithCharacteristics(
                                    new LoggingCharacteristics(log,
                                        new DefaultOnErrorCharacteristics(new KeyValuedAvitoCharacteristics(page))),
                                    new AvitoVehicleWithBrand(
                                        new LoggingBrandSource(log,
                                            new DefaultOnErrorBrandSource(new FromCharacteristicsBrandSource(page))),
                                        new AvitoVehicleWithKind(
                                            new LoggingKindSource(log,
                                                new DefaultOnErrorKindSource(new FromCharacteristicsKindSource(page))),
                                            new AvitoVehicleWithModel(
                                                new LoggingModelSource(log,
                                                    new DefaultOnErrorModel(new FromCharacteristicsModelSource(page))),
                                                new AvitoVehicleWithPrice(
                                                    new LoggingVehiclePriceSource(log,
                                                        new DefaultVehiclePriceOnErrorSource(
                                                            new VarietAvitoVehiclePriceSource()
                                                                .With(new AvitoVehiclePriceSource(page))
                                                                .With(new AvitoPriceFromContainer(page)))),
                                                    new BottomScrollingAvitoVehicle(bottomScroll,
                                                        new BlockBypassingAvitoVehicle(bypass,
                                                            new NavigatingAvitoVehicle(
                                                                new LoggingPageNavigating(log, item.ReadUrl(),
                                                                    new PageNavigating(page, item.ReadUrl())),
                                                                new EmptyAvitoVehicle())))))))))))).VehicleSource();
        }
    }
}