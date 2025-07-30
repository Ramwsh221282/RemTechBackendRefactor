using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCatalogueLocations.Types;

public sealed class CatalogueGeoLocationsReader(DbDataReader reader) : IAsyncDisposable
{
    public async Task<IEnumerable<CatalogueGeoLocationPresentation>> Read(
        CancellationToken ct = default
    )
    {
        LinkedList<CatalogueGeoLocationPresentation> locations = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal(string.Intern("location_id")));
            string name = reader.GetString(reader.GetOrdinal(string.Intern("location_name")));
            string kind = reader.GetString(reader.GetOrdinal(string.Intern("location_kind")));
            CatalogueGeoLocationPresentation location = new(id, name, kind);
            locations.AddFirst(location);
        }
        return locations.OrderBy(l => l.Name);
    }

    public ValueTask DisposeAsync()
    {
        return reader.DisposeAsync();
    }
}
