using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class KindedVehicle : VehicleEnvelope
{
    public KindedVehicle(Guid? id, string? name, IVehicle origin)
        : this(new NotEmptyGuid(id), new NotEmptyString(name), origin) { }

    public KindedVehicle(Guid? id, string? name)
        : this(new NotEmptyGuid(id), new NotEmptyString(name), new VehicleBlueprint()) { }

    public KindedVehicle(NotEmptyGuid id, NotEmptyString name, IVehicle origin)
        : this(new ExistingVehicleKind(id, name), origin) { }

    public KindedVehicle(NotEmptyGuid id, NotEmptyString name)
        : this(new ExistingVehicleKind(id, name), new VehicleBlueprint()) { }

    public KindedVehicle(string? name)
        : this(name, new VehicleBlueprint()) { }

    public KindedVehicle(string? name, IVehicle origin)
        : this(new NotEmptyString(name), origin) { }

    public KindedVehicle(NotEmptyString name, IVehicle origin)
        : this(new NewVehicleKind(name), origin) { }

    public KindedVehicle(NotEmptyString name)
        : this(new NewVehicleKind(name), new VehicleBlueprint()) { }

    public KindedVehicle(IVehicleKind kind, IVehicle origin)
        : base(
            origin.Identity(),
            kind,
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
