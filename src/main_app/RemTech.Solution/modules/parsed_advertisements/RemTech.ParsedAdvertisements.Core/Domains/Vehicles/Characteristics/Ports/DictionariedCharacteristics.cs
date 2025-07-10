using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;

public sealed class DictionariedCharacteristics : ICharacteristics
{
    private readonly Dictionary<NotEmptyString, CharacteristicEnvelope> _items = [];

    public Status<CharacteristicEnvelope> Add(string? name)
    {
        NotEmptyString ctxName = new(name);
        if (_items.TryGetValue(ctxName, out CharacteristicEnvelope? ctx))
            return ctx;
        NewCharacteristic characteristic = new(ctxName);
        _items.Add(ctxName, characteristic);
        return characteristic;
    }

    public MaybeBag<CharacteristicEnvelope> GetByName(string? name)
    {
        NotEmptyString ctxName = new(name);
        return _items.TryGetValue(ctxName, out CharacteristicEnvelope? ctx)
            ? ctx
            : new MaybeBag<CharacteristicEnvelope>();
    }
}
