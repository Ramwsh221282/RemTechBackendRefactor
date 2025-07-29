using System.Data.Common;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleModel(DbDataReader reader)
{
    public VehicleModel Read()
    {
        Guid modelId = reader.GetGuid(reader.GetOrdinal("model_id"));
        string modelName = reader.GetString(reader.GetOrdinal("model_name"));
        VehicleModelIdentity identity = new(modelId);
        VehicleModelName name = new(modelName);
        return new VehicleModel(identity, name);
    }
}
