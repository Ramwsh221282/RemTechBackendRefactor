using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

public sealed class VehicleKindPresentationReader(DbDataReader reader) : IAsyncDisposable
{
    public async Task<IEnumerable<VehicleKindPresentation>> Read(CancellationToken ct)
    {
        LinkedList<VehicleKindPresentation> presents = [];
        while (await reader.ReadAsync(ct))
        {
            string text = reader.GetString(reader.GetOrdinal("kind_name"));
            Guid id = reader.GetGuid(reader.GetOrdinal("kind_id"));
            int brandsCount = reader.GetInt32(reader.GetOrdinal("brands_count"));
            int modelsCount = reader.GetInt32(reader.GetOrdinal("models_count"));
            int vehiclesCount = reader.GetInt32(reader.GetOrdinal("vehicles_count"));
            VehicleKindPresentation item = new(id, text, brandsCount, modelsCount, vehiclesCount);
            presents.AddFirst(item);
        }
        return presents.OrderBy(v => v.Name);
    }

    public async ValueTask DisposeAsync() => await reader.DisposeAsync();
}
