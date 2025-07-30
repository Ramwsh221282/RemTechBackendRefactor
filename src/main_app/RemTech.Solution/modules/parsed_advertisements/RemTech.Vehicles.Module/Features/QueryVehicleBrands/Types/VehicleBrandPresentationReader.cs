using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

public sealed class VehicleBrandPresentationReader(DbDataReader reader) : IAsyncDisposable
{
    public async Task<IEnumerable<VehicleBrandPresentation>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleBrandPresentation> presents = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("brand_id"));
            string name = reader.GetString(reader.GetOrdinal("brand_name"));
            int modelsCount = reader.GetInt32(reader.GetOrdinal("models_count"));
            int vehiclesCount = reader.GetInt32(reader.GetOrdinal("vehicles_count"));
            VehicleBrandPresentation brand = new(id, name, modelsCount, vehiclesCount);
            presents.AddFirst(brand);
        }
        return presents.OrderBy(p => p.Name).ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        await reader.DisposeAsync();
    }
}
