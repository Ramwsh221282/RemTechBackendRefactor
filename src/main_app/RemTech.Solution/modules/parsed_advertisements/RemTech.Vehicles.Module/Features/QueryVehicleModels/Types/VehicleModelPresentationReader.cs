using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

public sealed record VehicleModelPresentationReader(DbDataReader Reader)
{
    public async Task<IEnumerable<VehicleModelPresentation>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleModelPresentation> presents = [];
        while (await Reader.ReadAsync(ct))
        {
            Guid id = Reader.GetGuid(Reader.GetOrdinal("id"));
            string text = Reader.GetString(Reader.GetOrdinal("text"));
            presents.AddFirst(new VehicleModelPresentation(id, text));
        }
        return presents.OrderBy(x => x.Name).ToArray();
    }
}
