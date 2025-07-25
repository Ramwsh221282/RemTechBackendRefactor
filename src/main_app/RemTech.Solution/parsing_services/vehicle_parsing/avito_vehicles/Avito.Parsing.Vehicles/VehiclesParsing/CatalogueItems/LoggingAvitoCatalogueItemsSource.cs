using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class LoggingAvitoCatalogueItemsSource(
    ICustomLogger log, 
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