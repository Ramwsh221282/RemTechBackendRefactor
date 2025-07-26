using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentModelInfo
{
    private readonly Guid _modelId;
    private readonly string _modelName;

    public VehiclePresentModelInfo()
    {
        _modelId = Guid.Empty;
        _modelName = string.Empty;
    }

    private VehiclePresentModelInfo(Guid id, string name)
    {
        _modelId = id;
        _modelName = name;
    }

    public VehiclePresentModelInfo RiddenBy(DbDataReader reader)
    {
        Guid modelId = reader.GetGuid(reader.GetOrdinal("model_id"));
        string modelName = reader.GetString(reader.GetOrdinal("model_name"));
        return new VehiclePresentModelInfo(modelId, modelName);
    }
}