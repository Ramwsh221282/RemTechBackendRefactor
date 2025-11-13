using RemTech.Functional.Extensions;

namespace Identity.Core.PermissionsModule.Contracts;

public sealed record PermissionsQueryArgs(
    Guid? Id = null, 
    string? Name = null);

public sealed record PermissionsStorage(
    Insert Insert, 
    Delete Delete, 
    Update Update, 
    Find Find, 
    FindMany FindMany);

public delegate Task<Result<Unit>> Insert(Permission permission, CancellationToken ct);
public delegate Task<Result<Unit>> Delete(Permission permission, CancellationToken ct);
public delegate Task<Result<Unit>> Update(Permission permission, CancellationToken ct);
public delegate Task<Optional<Permission>> Find(PermissionsQueryArgs args, CancellationToken ct);
public delegate Task<IEnumerable<Permission>> FindMany(PermissionsQueryArgs args, CancellationToken ct);