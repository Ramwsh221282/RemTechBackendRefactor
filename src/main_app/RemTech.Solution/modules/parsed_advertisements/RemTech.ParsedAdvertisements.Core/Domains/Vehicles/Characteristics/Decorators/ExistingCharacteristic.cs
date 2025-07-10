using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;

public sealed class ExistingCharacteristic : CharacteristicEnvelope
{
    public ExistingCharacteristic(NotEmptyGuid id, NotEmptyString name)
        : base(
            new CharacteristicIdentity(
                new CharacteristicId(id),
                new NewCharacteristic(name).Identify().ReadText()
            )
        ) { }

    public ExistingCharacteristic(Guid? id, string? name)
        : this(new NotEmptyGuid(id), new NotEmptyString(name)) { }
}
