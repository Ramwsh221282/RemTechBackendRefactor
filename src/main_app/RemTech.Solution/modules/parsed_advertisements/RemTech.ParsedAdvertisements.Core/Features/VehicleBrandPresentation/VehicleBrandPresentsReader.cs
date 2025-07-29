using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Features.VehicleBrandPresentation;

public sealed class VehicleBrandPresentsReader
{
    private readonly DbDataReader _reader;

    public VehicleBrandPresentsReader(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<IEnumerable<VehicleBrandPresent>> ReadAsync(CancellationToken ct)
    {
        LinkedList<VehicleBrandPresent> presents = [];
        while (await _reader.ReadAsync(ct))
        {
            Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
            string name = _reader.GetString(_reader.GetOrdinal("text"));
            presents.AddFirst(new VehicleBrandPresent(id, name));
        }
        return presents.OrderBy(p => p.Name).ToArray();
    }
}
