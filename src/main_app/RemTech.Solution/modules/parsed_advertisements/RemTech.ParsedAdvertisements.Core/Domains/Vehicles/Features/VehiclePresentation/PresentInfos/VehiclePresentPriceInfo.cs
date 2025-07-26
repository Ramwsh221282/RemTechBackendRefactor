using System.Data.Common;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentPriceInfo
{
    private readonly long _price;
    private readonly bool _isNds;
    public VehiclePresentPriceInfo() => _price = 1;

    private VehiclePresentPriceInfo(long price, bool isNds)
    {
        _price = price;
        _isNds = isNds;
    }

    public VehiclePresentPriceInfo RiddenBy(DbDataReader reader)
    {
        long price = reader.GetInt64(reader.GetOrdinal("vehicle_price"));
        bool isNds = reader.GetBoolean(reader.GetOrdinal("vehicle_nds"));
        return new VehiclePresentPriceInfo(price, isNds);
    }
}