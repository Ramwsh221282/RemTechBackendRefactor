using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentLocationInfo
{
    private readonly Guid _locationId;
    private readonly string _locationName;

    public VehiclePresentLocationInfo()
    {
        _locationId = Guid.Empty;
        _locationName = string.Empty;
    }

    private VehiclePresentLocationInfo(Guid id, string name)
    {
        _locationId = id;
        _locationName = name;
    }

    public VehiclePresentLocationInfo RiddenBy(DbDataReader reader)
    {
        Guid locationId = reader.GetGuid(reader.GetOrdinal("geo_id"));
        string locationName = reader.GetString(reader.GetOrdinal("geo_text"));
        return new VehiclePresentLocationInfo(locationId, locationName);
    }
}