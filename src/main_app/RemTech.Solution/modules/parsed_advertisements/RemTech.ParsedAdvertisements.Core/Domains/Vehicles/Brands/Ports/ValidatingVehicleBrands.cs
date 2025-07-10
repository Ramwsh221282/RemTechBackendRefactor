using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public sealed class ValidatingVehicleBrands(IVehicleBrands origin) : IVehicleBrands
{
    public Status<VehicleBrandEnvelope> Add(string? name)
    {
        NotEmptyString brandName = new(name);
        if (!brandName)
            return new ValidationError<VehicleBrandEnvelope>(
                $"Некорректное название бренда: {(string)brandName}"
            );
        return origin.Add(brandName);
    }

    public MaybeBag<VehicleBrandEnvelope> GetByName(string? name)
    {
        return origin.GetByName(name);
    }
}