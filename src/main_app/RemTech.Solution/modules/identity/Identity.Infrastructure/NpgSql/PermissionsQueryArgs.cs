namespace Identity.Infrastructure.NpgSql;

public sealed record PermissionsQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    bool WithLock = false);