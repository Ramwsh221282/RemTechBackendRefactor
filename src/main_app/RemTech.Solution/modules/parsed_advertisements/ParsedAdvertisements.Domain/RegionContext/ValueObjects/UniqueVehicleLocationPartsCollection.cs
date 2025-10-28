using RemTech.Core.Shared.Enumerable;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.RegionContext.ValueObjects;

public sealed class UniqueVehicleLocationPartsCollection
{
    private readonly IEnumerable<string> _parts;

    private UniqueVehicleLocationPartsCollection(IEnumerable<string> parts)
    {
        _parts = [..parts];
    }

    private UniqueVehicleLocationPartsCollection(IEnumerable<RegionName> parts)
    {
        _parts = [..parts.Select(p => p.Value)];
    }

    public static Status<UniqueVehicleLocationPartsCollection> Create(IEnumerable<string> parts)
    {
        var results = parts.Select(RegionName.Create).ToArray();
        var collection = new StatusCollection<RegionName>(results);
        if (!collection.AllValid(out var error, out var value))
            return error;
        return Create(value);
    }

    public static Status<UniqueVehicleLocationPartsCollection> Create(IEnumerable<RegionName> names)
    {
        var array = names.ToArray();
        if (!array.AllUnique(n => n.Value))
            return Error.Validation("Список локаций техники должен быть уникальным.");
        return new UniqueVehicleLocationPartsCollection(array);
    }
}