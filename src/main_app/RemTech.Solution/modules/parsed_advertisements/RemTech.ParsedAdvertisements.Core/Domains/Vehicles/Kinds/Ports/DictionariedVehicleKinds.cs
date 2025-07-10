using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public sealed class DictionariedVehicleKinds : IVehicleKinds
{
    private readonly Dictionary<NotEmptyString, VehicleKindEnvelope> _items = [];

    public Status<VehicleKindEnvelope> Add(string? name)
    {
        NotEmptyString kindName = new(name);
        if (_items.TryGetValue(kindName, out VehicleKindEnvelope? result))
            return result;
        NewVehicleKind created = new(kindName);
        _items.Add(kindName, created);
        return created;
    }

    public MaybeBag<VehicleKindEnvelope> GetByName(string? name)
    {
        NotEmptyString kindName = new(name);
        return _items.TryGetValue(kindName, out VehicleKindEnvelope? result)
            ? result
            : new MaybeBag<VehicleKindEnvelope>();
    }
}
