using Parsing.SDK.Logging;

namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class LoggingAvitoCatalogueItemsSource(
    IParsingLog log, 
    IAvitoCatalogueItemsSource items)
    : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        log.Info("Extracting avito catalogue items.");
        CatalogueItemsList extracted = await items.Read();
        log.Info("Extracted amount: {0}.", extracted.Amount());
        return extracted;
    }
}