namespace RemTech.ParsedAdvertisements.Core.Features.VehicleBrandPresentation;

public sealed class VehicleBrandPresent
{
    public Guid Id { get; }
    public string Name { get; }

    public VehicleBrandPresent(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
