using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands;

public sealed class SingleVehicleBrandSqlRow
{
    private readonly DbDataReader _reader;

    public SingleVehicleBrandSqlRow(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<MaybeBag<IVehicleBrand>> Read(CancellationToken ct = default)
    {
        if (!await _reader.ReadAsync(ct))
            return new MaybeBag<IVehicleBrand>();
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ExistingVehicleBrand(id, text);
    }
}
