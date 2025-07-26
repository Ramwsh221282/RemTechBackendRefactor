using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

public sealed class VehicleModelName
{
    private readonly NotEmptyString _name;

    public VehicleModelName() => _name = new NotEmptyString(string.Empty);
    public VehicleModelName(NotEmptyString name) => _name = name;
    
    public VehicleModelName(string name) : this(new NotEmptyString(name)) { }

    public static implicit operator string(VehicleModelName name) => name._name;
    public static implicit operator NotEmptyString(VehicleModelName name) => name._name;
    public static implicit operator bool(VehicleModelName name) => name._name;
}