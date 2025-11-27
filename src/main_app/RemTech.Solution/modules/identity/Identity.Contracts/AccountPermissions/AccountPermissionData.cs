namespace Identity.Contracts.AccountPermissions;

public sealed record AccountPermissionData(
    Guid AccountId, 
    Guid PermissionId,
    string Email,
    string AccountName,
    string PermissionName);