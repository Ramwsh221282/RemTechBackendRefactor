using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.Entities;

public sealed record VehicleCharacteristic
{
    public Guid VehicleId { get; }
    public Guid CharacteristicId { get; }

    private VehicleCharacteristic(Guid vehicleId, Guid characteristicId) =>
        (VehicleId, CharacteristicId) = (vehicleId, characteristicId);

    public static Status<VehicleCharacteristic> Create(Guid vehicleId, Guid characteristicId)
    {
        if (vehicleId == Guid.Empty)
            return Error.Validation(
                "Характеристика техники не может иметь пустой идентификатор техники"
            );

        if (characteristicId == Guid.Empty)
            return Error.Validation(
                "Характеристика техники не может иметь пустой идентификатор характеристики"
            );

        return new VehicleCharacteristic(vehicleId, characteristicId);
    }
}
