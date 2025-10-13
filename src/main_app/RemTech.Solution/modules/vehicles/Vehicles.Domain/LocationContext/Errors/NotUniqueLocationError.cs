using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record NotUniqueLocationError : Error
{
    public NotUniqueLocationError(Location location)
        : this(location.Address) { }

    public NotUniqueLocationError(LocationAddress address)
        : this(address.Parts) { }

    public NotUniqueLocationError(IEnumerable<LocationAddressPart> parts)
        : base(
            $"Локация с адресом: {string.Join(" ,", parts.Select(p => p.Value))} уже существует.",
            ErrorCodes.Validation
        ) { }
}
