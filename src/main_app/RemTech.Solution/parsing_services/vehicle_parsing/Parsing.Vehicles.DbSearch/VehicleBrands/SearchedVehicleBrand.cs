using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public sealed class SearchedVehicleBrand
{
    private readonly DbDataReader _reader;

    public SearchedVehicleBrand(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<ParsedVehicleBrand> Read()
    {
        if (!await _reader.ReadAsync())
            return new ParsedVehicleBrand(new NotEmptyString(string.Empty));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ParsedVehicleBrand(text);
    }
}