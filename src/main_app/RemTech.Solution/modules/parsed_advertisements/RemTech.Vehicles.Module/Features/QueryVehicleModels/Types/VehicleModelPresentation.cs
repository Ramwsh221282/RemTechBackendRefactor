namespace RemTech.Vehicles.Module.Features.QueryVehicleModels.Types;

public sealed record VehicleModelPresentation(Guid Id, string Name)
{
    public Guid Id { get; } = Id;
    public string Name { get; } = Name;
}
