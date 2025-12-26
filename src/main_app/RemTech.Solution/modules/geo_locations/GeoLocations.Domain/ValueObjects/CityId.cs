using RemTech.Core.Shared.Result;

namespace GeoLocations.Domain.ValueObjects;

public readonly record struct CityId
{
    public Guid Value { get; }

    public CityId() => Value = Guid.NewGuid();

    private CityId(Guid id) => Value = id;

    public static Status<CityId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.Validation("Идентификатор города был пустым.")
            : new CityId(value);
}
