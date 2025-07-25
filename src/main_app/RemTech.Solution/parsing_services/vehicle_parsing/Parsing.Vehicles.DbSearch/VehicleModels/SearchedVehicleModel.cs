using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public sealed class SearchedVehicleModel
{
    private readonly DbDataReader _reader;

    public SearchedVehicleModel(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<ParsedVehicleModel> Read()
    {
        if (!await _reader.ReadAsync())
            return new  ParsedVehicleModel(new NotEmptyString(string.Empty));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ParsedVehicleModel(text);
    }
}