using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Decorators;

public sealed class ValidatingVehicleKindBakery : IVehicleKindBakery
{
    private readonly IVehicleKindBakery _origin;

    public ValidatingVehicleKindBakery(IVehicleKindBakery origin)
    {
        _origin = origin;
    }

    public Status<IVehicleKind> Baked(IVehicleKindReceipt receipt)
    {
        NotEmptyGuid whatId = receipt.WhatId();
        NotEmptyString whatName = receipt.WhatName();
        if (!whatId)
            return new ValidationError<IVehicleKind>("Некорректный ID типа техники.");
        if (!whatName)
            return new ValidationError<IVehicleKind>("Некорректное название типа техники.");
        return _origin.Baked(receipt);
    }
}
