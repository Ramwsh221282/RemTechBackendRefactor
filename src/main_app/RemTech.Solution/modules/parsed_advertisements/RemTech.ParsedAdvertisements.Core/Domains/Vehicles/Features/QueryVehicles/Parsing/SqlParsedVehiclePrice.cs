using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Parsing;

public sealed class SqlParsedVehiclePrice(DbDataReader reader)
{
    public IItemPrice Read()
    {
        long price = reader.GetInt64(reader.GetOrdinal("vehicle_price"));
        bool isNds = reader.GetBoolean(reader.GetOrdinal("vehicle_nds"));
        PriceValue priceValue = new(price);
        return isNds ? new ItemPriceWithNds(priceValue) : new ItemPriceWithoutNds(priceValue);
    }
}