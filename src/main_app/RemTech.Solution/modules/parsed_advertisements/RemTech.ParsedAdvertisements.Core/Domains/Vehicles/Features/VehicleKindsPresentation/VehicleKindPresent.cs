namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;

public sealed record VehicleKindPresent
{
    public Guid Id { get; }
    public string Name { get; }

    public VehicleKindPresent(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}