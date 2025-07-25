using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class UniqueCharacteristics
{
    private readonly Dictionary<string, Characteristic> _ctxes = [];

    public UniqueCharacteristics With(NotEmptyString name, Characteristic ctx)
    {
        string nameString = name;
        if (_ctxes.ContainsKey(nameString)) return this;
        _ctxes.Add(nameString, ctx);
        return this;
    }

    public Characteristic[] ReadAll() => _ctxes.Values.ToArray();
}