using Identity.Contracts.Shared.Contracts;

namespace Identity.Contracts.Permissions;

public sealed record PermissionQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    bool WithLock = false) : EntityFetchArgs;