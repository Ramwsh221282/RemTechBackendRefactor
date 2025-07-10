using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class PricedVehicle : VehicleEnvelope
{
    public PricedVehicle(long? value, string? extra)
        : this(value, extra, new VehicleBlueprint()) { }

    public PricedVehicle(long? value, string? extra, IVehicle origin)
        : this(new PositiveLong(value), new NotEmptyString(extra), origin) { }

    public PricedVehicle(PositiveLong value, NotEmptyString extra, IVehicle origin)
        : this(new PriceValue(value), extra, origin) { }

    public PricedVehicle(PositiveLong value, NotEmptyString extra)
        : this(new PriceValue(value), extra, new VehicleBlueprint()) { }

    public PricedVehicle(PriceValue value, NotEmptyString extra, IVehicle origin)
        : this(
            ((string)extra).Contains("НДС", StringComparison.OrdinalIgnoreCase)
                ? new ItemPrice(new ItemPriceWithNds(value))
                : new ItemPrice(new ItemPriceWithoutNds(value)),
            origin
        ) { }

    public PricedVehicle(IItemPrice price, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            price,
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
