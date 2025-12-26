using ParsedAdvertisements.Core.CharacteristicContext;

namespace ParsedAdvertisements.Core.VehicleContext.ValueObjects;

public sealed record VehicleCharacteristic(Characteristic Characteristic, string Value);