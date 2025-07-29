using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.VehicleKindsPresentation;

public sealed class VehicleKindPresentReader
{
    private readonly DbDataReader _reader;

    public VehicleKindPresentReader(DbDataReader reader) => _reader = reader;

    public async Task<IEnumerable<VehicleKindPresent>> Read(CancellationToken ct)
    {
        LinkedList<VehicleKindPresent> presents = [];
        while (await _reader.ReadAsync(ct))
        {
            string text = _reader.GetString(_reader.GetOrdinal("text"));
            Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
            presents.AddFirst(new VehicleKindPresent(id, text));
        }

        return presents.OrderBy(v => v.Name).ToArray();
    }
}
