using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Characteristics;

public sealed class Characteristic(CharacteristicId id, CharacteristicName name) : IPersistable<Characteristic>
{
    public CharacteristicId Id { get; } = id;
    public CharacteristicName Name { get; } = name;

    public Task<Result<Characteristic>> SaveBy(IPersister persister, CancellationToken ct = default) =>
        persister.Save(this, ct);
}
