namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class EmptyAvitoCatalogueItemsSource : IAvitoCatalogueItemsSource
{
    public Task<CatalogueItemsList> Read()
    {
        return Task.FromResult(new CatalogueItemsList());
    }
}
