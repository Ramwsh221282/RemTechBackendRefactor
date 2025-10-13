using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Errors;

namespace Vehicles.Domain.LocationContext.ValueObjects;

public sealed record LocationId
{
    public Guid Value { get; }

    public LocationId() => Value = Guid.NewGuid();

    private LocationId(Guid value) => Value = value;

    public static Result<LocationId> Create(Guid value) =>
        value == Guid.Empty ? new LocationIdEmptyError() : new LocationId(value);

    public static Result<LocationId> Create(Guid? value) =>
        value == null ? new LocationIdEmptyError() : new LocationId(value.Value);
}
