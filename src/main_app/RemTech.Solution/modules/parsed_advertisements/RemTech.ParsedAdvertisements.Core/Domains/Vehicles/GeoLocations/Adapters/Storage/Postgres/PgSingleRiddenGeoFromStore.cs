using System.Data.Common;
using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;

public sealed class PgSingleRiddenGeoFromStore
{
    private readonly DbDataReader _reader;

    public PgSingleRiddenGeoFromStore(DbDataReader reader) =>
        _reader = reader;

    public async Task<GeoLocation> Read()
    {
        if (!await _reader.ReadAsync())
            throw new OperationException("Геолокация не найдена.");
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string name = _reader.GetString(_reader.GetOrdinal("text"));
        string kind = _reader.GetString(_reader.GetOrdinal("kind"));
        return new ExistingGeoLocation(id, name, kind);
    }
}