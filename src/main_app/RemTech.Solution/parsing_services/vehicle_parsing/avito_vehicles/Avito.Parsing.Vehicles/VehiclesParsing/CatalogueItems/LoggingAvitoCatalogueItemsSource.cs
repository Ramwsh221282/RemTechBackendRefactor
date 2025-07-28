using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class LoggingAvitoCatalogueItemsSource(
    ILogger log, 
    IAvitoCatalogueItemsSource items)
    : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        log.Information("Extracting avito catalogue items.");
        CatalogueItemsList extracted = await items.Read();
        log.Information("Extracted amount: {0}.", extracted.Amount());
        return extracted;
    }
}