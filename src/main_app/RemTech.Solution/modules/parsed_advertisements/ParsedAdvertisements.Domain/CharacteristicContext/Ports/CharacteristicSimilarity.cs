using ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;
using ParsedAdvertisements.Domain.VehicleContext.Entities;
using ParsedAdvertisements.Domain.VehicleContext.ValueObjects;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.CharacteristicContext.Ports;

public sealed class CharacteristicSimilarity(CharacteristicName origin, string value)
{
    public CharacteristicName Origin { get; } = origin;
    public string Value { get; } = value;
    public CharacteristicId? MatchedId { get; }
    public CharacteristicName? Matched { get; }

    public CharacteristicSimilarity(
        CharacteristicName origin,
        CharacteristicId? matchedId,
        CharacteristicName matched,
        string value) : this(origin, value) =>
        (MatchedId, Matched) = (matchedId, matched);

    public Status<VehicleCharacteristic> ToVehicleCharacteristic(VehicleIdentitySpecification identity)
    {
        if (MatchedId == null || Matched == null)
            return Error.Conflict("Не удается распознать характеристику техники.");
        return VehicleCharacteristic.Create(identity.VehicleId, MatchedId.Value.Value, Matched.Value, Value);
    }
}