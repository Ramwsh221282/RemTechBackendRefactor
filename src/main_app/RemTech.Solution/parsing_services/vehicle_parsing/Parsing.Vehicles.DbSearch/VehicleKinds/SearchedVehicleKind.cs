using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class SearchedVehicleKind
{
    private readonly DbDataReader _reader;

    public SearchedVehicleKind(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<ParsedVehicleKind> Read()
    {
        if (!await _reader.ReadAsync())
            throw new ArgumentException("No vehicle kinds found.");
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        return new ParsedVehicleKind(text);
    }
}
