using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Types.Brands;
using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleBrand(DbDataReader reader)
{
    public VehicleBrand Read()
    {
        Guid brandId = reader.GetGuid(reader.GetOrdinal("brand_id"));
        string brandName = reader.GetString(reader.GetOrdinal("brand_name"));
        VehicleBrandIdentity identity = new(
            new VehicleBrandId(brandId),
            new VehicleBrandText(brandName)
        );
        return new VehicleBrand(identity);
    }
}
