using RemTech.Core.Shared.Result;

namespace GeoLocations.Domain.ValueObjects;

public readonly record struct LocationRegionId
{
    public Guid Value { get; }

    public LocationRegionId() => Value = Guid.NewGuid();

    private LocationRegionId(Guid id) => Value = id;

    public static Status<LocationRegionId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Идентификатор региона был пустым.")
            : new LocationRegionId(id);
}
