namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class EmptyAvitoCatalogueItemsSource : IAvitoCatalogueItemsSource
{
    public Task<CatalogueItemsList> Read()
    {
        return Task.FromResult(new CatalogueItemsList());
    }
}