using System.Text.Json;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehiclePhoto(JsonElement photoJson)
{
    public VehiclePhoto Read()
    {
        string? source = photoJson.GetString();
        return string.IsNullOrEmpty(source)
            ? throw new ApplicationException(
                $"{nameof(SqlParsedVehiclePhoto)} vehicle photo source is empty."
            )
            : new VehiclePhoto(source);
    }
}
