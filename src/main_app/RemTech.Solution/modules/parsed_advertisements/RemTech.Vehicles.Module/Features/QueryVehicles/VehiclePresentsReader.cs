using System.Data.Common;
using RemTech.Vehicles.Module.Features.QueryVehicles.Presenting;

namespace RemTech.Vehicles.Module.Features.QueryVehicles;

public sealed class VehiclePresentsReader(DbDataReader reader) : IAsyncDisposable
{
    public async Task<IEnumerable<VehiclePresentation>> Read(CancellationToken ct = default)
    {
        List<VehiclePresentation> results = [];
        while (await reader.ReadAsync(ct))
            results.Add(VehiclePresentation.FromReader(reader));
        return results;
    }

    public async ValueTask DisposeAsync()
    {
        await reader.DisposeAsync();
    }
}
