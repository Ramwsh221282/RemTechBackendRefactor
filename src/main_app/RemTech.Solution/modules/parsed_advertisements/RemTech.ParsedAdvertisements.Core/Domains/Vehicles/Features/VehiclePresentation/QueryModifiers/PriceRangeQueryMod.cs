namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class PriceRangeQueryMod : IVehiclePresentQueryMod
{
    private readonly long? _startFrom;
    private readonly long? _endAt;

    public PriceRangeQueryMod(long? startFrom, long? endAt)
    {
        _startFrom = startFrom;
        _endAt = endAt;
    }
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        if (_startFrom.HasValue)
            storage = storage.Put("v.price >= @startFrom", "@startFrom", _startFrom.Value);
        if (_endAt.HasValue)
            storage = storage.Put("v.price <= @endAt", "@endAt", _endAt.Value);
        return storage;
    }
}