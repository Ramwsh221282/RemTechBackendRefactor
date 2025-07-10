using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public sealed class DictionariedVehicleBrands : IVehicleBrands
{
    private readonly Dictionary<NotEmptyString, VehicleBrandEnvelope> _items = [];

    public Status<VehicleBrandEnvelope> Add(string? name)
    {
        NotEmptyString brandName = new(name);
        if (_items.TryGetValue(brandName, out VehicleBrandEnvelope? envelope))
            return envelope;
        NewVehicleBrand created = new(brandName);
        _items.Add(brandName, created);
        return created;
    }

    public MaybeBag<VehicleBrandEnvelope> GetByName(string? name)
    {
        NotEmptyString brandName = new(name);
        return _items.TryGetValue(brandName, out VehicleBrandEnvelope? envelope)
            ? envelope
            : new MaybeBag<VehicleBrandEnvelope>();
    }
}
