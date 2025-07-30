using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

public sealed class VehicleModelPresentationReader(DbDataReader reader) : IAsyncDisposable
{
    public async Task<IEnumerable<VehicleModelPresentation>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleModelPresentation> presents = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("model_id"));
            string text = reader.GetString(reader.GetOrdinal("model_name"));
            int vehiclesCount = reader.GetInt32(reader.GetOrdinal("vehicles_count"));
            VehicleModelPresentation model = new(id, text, vehiclesCount);
            presents.AddFirst(model);
        }
        return presents.OrderBy(x => x.Name).ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        await reader.DisposeAsync();
    }
}
