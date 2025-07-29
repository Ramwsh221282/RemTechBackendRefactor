namespace RemTech.Vehicles.Module.Features.VehicleModelsPresentation;

public sealed class VehicleModelPresent
{
    public Guid Id { get; }
    public string Name { get; }

    public VehicleModelPresent(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
