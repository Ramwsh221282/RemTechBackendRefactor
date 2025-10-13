using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Domain.LocationContext;

public sealed class UniqueLocation
{
    private readonly LocationAddress? _address;

    public UniqueLocation(LocationAddress? address) => _address = address;

    public Result<Location> Unique(Location location) =>
        _address != null && _address.IsSameAs(location)
            ? new NotUniqueLocationError(location)
            : location;
}
