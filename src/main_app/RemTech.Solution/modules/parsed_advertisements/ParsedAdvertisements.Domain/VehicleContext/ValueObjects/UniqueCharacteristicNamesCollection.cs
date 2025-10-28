using ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;
using RemTech.Core.Shared.Enumerable;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed class UniqueCharacteristicNamesCollection
{
    private readonly IEnumerable<CharacteristicName> _names;

    private UniqueCharacteristicNamesCollection(IEnumerable<CharacteristicName> names)
    {
        _names = [..names];
    }

    public static Status<UniqueCharacteristicNamesCollection> Create(IEnumerable<string> names)
    {
        var results = names.Select(CharacteristicName.Create).ToArray();
        var collection = new StatusCollection<CharacteristicName>(results);
        if (!collection.AllValid(out var error, out var value))
            return error;
        return Create(value);
    }

    public static Status<UniqueCharacteristicNamesCollection> Create(IEnumerable<CharacteristicName> names)
    {
        var array = names.ToArray();
        if (!array.AllUnique(c => c.Value))
            return Error.Validation("Список названий характеристик техники должен быть уникальным.");
        return new UniqueCharacteristicNamesCollection(array);
    }
}