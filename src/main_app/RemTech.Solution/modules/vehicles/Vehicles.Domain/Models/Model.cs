using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Models;

public sealed class Model(ModelId id, ModelName name) : IPersistable<Model>
{
    public ModelId Id { get; } = id;
    public ModelName Name { get; } = name;

    public Task<Result<Model>> SaveBy(IPersister persister, CancellationToken ct = default) => persister.Save(this, ct);
}
