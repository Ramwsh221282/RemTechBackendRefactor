using Identity.Contracts.Shared.Contracts;

namespace Identity.Contracts.AccountPermissions;

public sealed record AccountPermissionQueryArgs(
    Guid? PermissionId = null, 
    Guid? AccountId = null,
    string? PermissionName = null,
    string? AccountName = null,
    string? AccountEmail = null,
    bool WithLock = false) : EntityFetchArgs;