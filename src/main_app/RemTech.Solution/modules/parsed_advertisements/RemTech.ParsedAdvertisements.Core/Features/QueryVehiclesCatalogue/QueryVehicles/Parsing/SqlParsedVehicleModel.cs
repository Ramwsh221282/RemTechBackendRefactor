using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Types.Models;
using RemTech.ParsedAdvertisements.Core.Types.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

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
