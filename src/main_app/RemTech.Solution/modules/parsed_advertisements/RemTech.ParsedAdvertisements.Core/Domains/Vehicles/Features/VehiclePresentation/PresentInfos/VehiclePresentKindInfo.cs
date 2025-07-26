using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentKindInfo
{
    private readonly Guid _kindId;
    private readonly string _kindName;

    public VehiclePresentKindInfo()
    {
        _kindId = Guid.Empty;
        _kindName = string.Empty;
    }

    private VehiclePresentKindInfo(Guid id, string name)
    {
        _kindId = id;
        _kindName = name;
    }

    public VehiclePresentKindInfo RiddenBy(DbDataReader reader)
    {
        Guid kindId = reader.GetGuid(reader.GetOrdinal("kind_id"));
        string kindName = reader.GetString(reader.GetOrdinal("kind_name"));
        return new VehiclePresentKindInfo(kindId, kindName);
    }
}