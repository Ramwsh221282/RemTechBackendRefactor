using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

public sealed class ExistingVehicleKind : VehicleKindEnvelope
{
    public ExistingVehicleKind(Guid? id, string? name)
        : this(new NotEmptyGuid(id), new NotEmptyString(name)) { }

    public ExistingVehicleKind(NotEmptyGuid id, NotEmptyString name)
        : base(
            new VehicleKindIdentity(
                new VehicleKindId(id),
                new NewVehicleKind(name).Identify().ReadText()
            )
        ) { }
}
