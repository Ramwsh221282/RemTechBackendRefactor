using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class MaybeSingleGeoLocationSqlRow
{
    private readonly DbDataReader _reader;

    public MaybeSingleGeoLocationSqlRow(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<MaybeBag<IGeoLocation>> Read(CancellationToken ct = default)
    {
        if (!await _reader.ReadAsync(ct))
            return new MaybeBag<IGeoLocation>();
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ExistingGeoLocation(id, text);
    }
}