using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.Entities;

public sealed record VehicleCharacteristic
{
    public string VehicleId { get; }
    public Guid CharacteristicId { get; }
    public string CharacteristicName { get; }
    public string CharacteristicValue { get; }

    private VehicleCharacteristic(
        string vehicleId,
        Guid characteristicId,
        string characteristicName,
        string characteristicValue) =>
        (VehicleId, CharacteristicId, CharacteristicName, CharacteristicValue) =
        (vehicleId, characteristicId, characteristicName, characteristicValue);

    public static Status<VehicleCharacteristic> Create(
        string vehicleId,
        Guid characteristicId,
        string characteristicName,
        string characteristicValue)
    {
        if (string.IsNullOrWhiteSpace(vehicleId))
        {
            var error = "Характеристика техники не может иметь пустой идентификатор техники";
            return Error.Validation(error);
        }

        if (characteristicId == Guid.Empty)
        {
            var error = "Характеристика техники не может иметь пустой идентификатор характеристики";
            return Error.Validation(error);
        }

        return new VehicleCharacteristic(
            vehicleId,
            characteristicId,
            characteristicName,
            characteristicValue);
    }
}