using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Models;

public class VehicleModel
{
    protected virtual VehicleModelIdentity Identity { get; }
    protected virtual VehicleModelName Name { get; }

    public VehicleModel(VehicleModelIdentity identity, VehicleModelName name)
    {
        Identity = identity;
        Name = name;
    }

    public VehicleModel()
    {
        Identity = new VehicleModelIdentity();
        Name = new VehicleModelName();
    }

    public VehicleModel(VehicleModel origin)
    {
        Identity = origin.Identity;
        Name = origin.Name;
    }

    private VehicleModel(VehicleModel origin, VehicleModelIdentity identity)
        : this(origin)
    {
        Identity = identity;
    }

    private VehicleModel(VehicleModel origin, VehicleModelName name)
        : this(origin)
    {
        Name = name;
    }

    public VehicleModel Rename(VehicleModelName name)
    {
        return new VehicleModel(this, name);
    }

    public VehicleModel ChangeIdentity(VehicleModelIdentity identity)
    {
        return new VehicleModel(this, identity);
    }

    public Guid Id() => Identity;

    public string NameString() => Name;
}
