using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Locations;

public sealed class Location(LocationId id, LocationName name) : IPersistable<Location>
{
    public LocationId Id { get; } = id;
    public LocationName Name { get; } = name;

    public Task<Result<Location>> SaveBy(IPersister persister, CancellationToken ct = default) =>
        persister.Save(this, ct);
}
