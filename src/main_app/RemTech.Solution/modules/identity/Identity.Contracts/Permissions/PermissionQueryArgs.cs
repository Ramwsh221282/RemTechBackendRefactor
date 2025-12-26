using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace Identity.Contracts.Permissions;

public sealed record PermissionQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    bool WithLock = false) : EntityFetchArgs;