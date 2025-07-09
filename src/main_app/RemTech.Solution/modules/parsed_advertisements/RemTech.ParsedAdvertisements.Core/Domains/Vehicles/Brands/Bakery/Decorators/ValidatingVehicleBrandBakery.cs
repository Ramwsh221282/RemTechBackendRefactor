using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Decorators;

public sealed class ValidatingVehicleBrandBakery : IVehicleBrandBakery
{
    private readonly IVehicleBrandBakery _origin;

    public ValidatingVehicleBrandBakery(IVehicleBrandBakery origin)
    {
        _origin = origin;
    }

    public Status<IVehicleBrand> Bake(IVehicleBrandReceipt receipt)
    {
        NotEmptyString name = receipt.WhatName();
        NotEmptyGuid id = receipt.WhatId();
        if (!name)
            return Status<IVehicleBrand>.Failure(
                new ValidationError($"Некорректное название бренда: {(string)name}")
            );
        if (!id)
            return Status<IVehicleBrand>.Failure(
                new ValidationError($"Некорректное ID бренда: {(Guid)id}")
            );
        return _origin.Bake(receipt);
    }
}
