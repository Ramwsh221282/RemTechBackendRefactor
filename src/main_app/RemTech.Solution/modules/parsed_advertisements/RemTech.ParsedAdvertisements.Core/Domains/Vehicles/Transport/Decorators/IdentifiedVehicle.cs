using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class IdentifiedVehicle : VehicleEnvelope
{
    public IdentifiedVehicle(VehicleId id, IVehicle origin)
        : this(new VehicleIdentity(id), origin) { }

    public IdentifiedVehicle(VehicleId id)
        : this(new VehicleIdentity(id), new VehicleBlueprint()) { }

    public IdentifiedVehicle(NotEmptyString id, IVehicle origin)
        : this(new VehicleIdentity(new VehicleId(id)), origin) { }

    public IdentifiedVehicle(NotEmptyString id)
        : this(new VehicleIdentity(new VehicleId(id)), new VehicleBlueprint()) { }

    public IdentifiedVehicle(string? id, IVehicle origin)
        : this(new VehicleIdentity(new VehicleId(id)), origin) { }

    public IdentifiedVehicle(string? id)
        : this(new VehicleIdentity(new VehicleId(id)), new VehicleBlueprint()) { }

    public IdentifiedVehicle(VehicleIdentity identity, IVehicle origin)
        : base(
            identity,
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
