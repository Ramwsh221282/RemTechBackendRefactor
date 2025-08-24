using Parsing.Grpc.Services.DuplicateIds;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class DuplicateFilteringAvitoCatalogueItems(
    GrpcDuplicateIdsClient client,
    IAvitoCatalogueItemsSource origin
) : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        CatalogueItemsList fromOrigin = await origin.Read();
        await fromOrigin.Filter(client);
        return fromOrigin;
    }
}
