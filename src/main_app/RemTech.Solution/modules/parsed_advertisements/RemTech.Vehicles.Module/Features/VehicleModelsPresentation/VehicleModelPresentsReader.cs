using System.Data.Common;

namespace RemTech.Vehicles.Module.Features.VehicleModelsPresentation;

public sealed class VehicleModelPresentsReader
{
    private readonly DbDataReader _reader;

    public VehicleModelPresentsReader(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<IEnumerable<VehicleModelPresent>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleModelPresent> presents = [];
        while (await _reader.ReadAsync(ct))
        {
            Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
            string text = _reader.GetString(_reader.GetOrdinal("text"));
            presents.AddFirst(new VehicleModelPresent(id, text));
        }
        return presents.OrderBy(x => x.Name).ToArray();
    }
}
