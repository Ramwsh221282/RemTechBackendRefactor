namespace Users.Module.Features.UpdateUserProfile;

internal sealed record UpdateUserProfileResult(
    Guid UserId,
    string UserEmail,
    string UserName,
    string UserRole
);
