using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation;

public sealed class VehiclePresentsReader
{
    private readonly DbDataReader _reader;

    public VehiclePresentsReader(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<IEnumerable<VehiclePresent>> Read(CancellationToken ct)
    {
        LinkedList<VehiclePresent> presents = [];
        while (await _reader.ReadAsync(ct))
            presents.AddFirst(new VehiclePresent().RiddenBy(_reader));
        return presents;
    }
}