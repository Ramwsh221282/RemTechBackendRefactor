using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;

namespace Vehicles.Domain.LocationContext.ValueObjects;

public sealed record LocationAddressPart
{
    public const int MaxLength = 50;
    public string Value { get; }

    private LocationAddressPart(string value) => Value = value;

    public static Result<LocationAddressPart> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new LocationAddressPartEmptyError();
        if (value.Length > MaxLength)
            return new LocationAddressPartExceesLengthError(value.Length);
        return new LocationAddressPart(value);
    }
}
