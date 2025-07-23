using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public sealed class SearchedRegionCity
{
    private readonly DbDataReader _reader;
    private readonly ParsedVehicleGeo _withRegion;

    public SearchedRegionCity(DbDataReader reader, ParsedVehicleGeo withRegion)
    {
        _reader = reader;
        _withRegion = withRegion;
    }

    public async Task<ParsedVehicleGeo> Read()
    {
        if (!await _reader.ReadAsync())
            return _withRegion;
        string region = _withRegion.Region();
        string city = _reader.GetString(_reader.GetOrdinal("city"));
        return new ParsedVehicleGeo(new ParsedVehicleRegion(region), new ParsedVehicleCity(city));
    }
}