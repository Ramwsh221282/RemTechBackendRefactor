namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class LocationQueryMod : IVehiclePresentQueryMod
{
    private Guid? _id;

    public LocationQueryMod(Guid? id) =>
        _id = id;
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        return _id.HasValue ? storage.Put("g.id = @geoid", "@geoid", _id.Value) : storage;
    }
}