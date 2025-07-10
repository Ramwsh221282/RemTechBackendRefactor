using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class CharacterizedVehicle : VehicleEnvelope
{
    public CharacterizedVehicle(IEnumerable<VehicleCharacteristic> characteristics)
        : this(characteristics, new VehicleBlueprint()) { }

    public CharacterizedVehicle(IEnumerable<VehicleCharacteristic> characteristics, IVehicle origin)
        : this(new VehicleCharacteristics(characteristics), origin) { }

    public CharacterizedVehicle(VehicleCharacteristics characteristics, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            characteristics
        ) { }
}
