using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class UniquePhotosVehicle : VehicleEnvelope
{
    public UniquePhotosVehicle(VehiclePhotos photos)
        : this(photos, new VehicleBlueprint()) { }

    public UniquePhotosVehicle(VehiclePhotos photos, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            photos,
            origin.Characteristics()
        ) { }
}
