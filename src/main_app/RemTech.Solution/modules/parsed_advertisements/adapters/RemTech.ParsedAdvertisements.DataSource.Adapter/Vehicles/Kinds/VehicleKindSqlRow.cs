using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

public sealed class VehicleKindSqlRow
{
    private readonly DbDataReader _reader;

    public VehicleKindSqlRow(DbDataReader reader)
    {
        _reader = reader;
    }

    public IVehicleKind Read()
    {
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ExistingVehicleKind(id, text);
    }
}
