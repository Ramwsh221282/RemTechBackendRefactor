namespace Users.Module.Features.UpdateUserProfile;

internal sealed record UpdateUserDetails(
    string? NewUserEmail,
    string? NewUserName,
    string? NewUserRole,
    bool IsPasswordUpdateRequired
);
