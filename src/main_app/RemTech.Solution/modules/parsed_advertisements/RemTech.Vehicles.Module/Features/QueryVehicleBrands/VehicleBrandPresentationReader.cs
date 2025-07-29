using System.Data.Common;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands;

public sealed record VehicleBrandPresentationReader(DbDataReader Reader)
{
    public async Task<IEnumerable<VehicleBrandPresentation>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleBrandPresentation> presents = [];
        while (await Reader.ReadAsync(ct))
        {
            Guid id = Reader.GetGuid(Reader.GetOrdinal("id"));
            string name = Reader.GetString(Reader.GetOrdinal("text"));
            presents.AddFirst(new VehicleBrandPresentation(id, name));
        }
        return presents.OrderBy(p => p.Name).ToArray();
    }
}
