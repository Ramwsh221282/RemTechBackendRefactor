using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;

public sealed record VehicleKindPresentationReader(DbDataReader Reader)
{
    public async Task<IEnumerable<VehicleKindPresentation>> Read(CancellationToken ct)
    {
        LinkedList<VehicleKindPresentation> presents = [];
        while (await Reader.ReadAsync(ct))
        {
            string text = Reader.GetString(Reader.GetOrdinal("text"));
            Guid id = Reader.GetGuid(Reader.GetOrdinal("id"));
            presents.AddFirst(new VehicleKindPresentation(id, text));
        }

        return presents.OrderBy(v => v.Name).ToArray();
    }
}
