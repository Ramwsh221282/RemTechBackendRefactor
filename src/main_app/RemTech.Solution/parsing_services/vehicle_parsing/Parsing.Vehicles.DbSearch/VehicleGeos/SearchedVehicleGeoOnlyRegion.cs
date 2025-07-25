using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class SearchedVehicleGeoOnlyRegion
{
    private readonly DbDataReader _reader;

    public SearchedVehicleGeoOnlyRegion(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<ParsedVehicleGeo> Read()
    {
        if (!await _reader.ReadAsync())
            return new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity());
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ParsedVehicleGeo(new ParsedVehicleRegion(text), new ParsedVehicleCity());
    }
}