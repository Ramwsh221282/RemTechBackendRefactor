using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentBrandInfo
{
    private readonly Guid _brandId;
    private readonly string _brandName;

    public VehiclePresentBrandInfo()
    {
        _brandId = Guid.Empty;
        _brandName = string.Empty;
    }

    private VehiclePresentBrandInfo(Guid id, string name)
    {
        _brandId = id;
        _brandName = name;
    }
    
    public VehiclePresentBrandInfo RiddenBy(DbDataReader reader)
    {
        Guid brandId = reader.GetGuid(reader.GetOrdinal("brand_id"));
        string brandName = reader.GetString(reader.GetOrdinal("brand_name"));
        return new VehiclePresentBrandInfo(brandId, brandName);
    }
}