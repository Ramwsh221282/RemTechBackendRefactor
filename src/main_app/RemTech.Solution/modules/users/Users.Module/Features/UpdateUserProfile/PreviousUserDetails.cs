namespace Users.Module.Features.UpdateUserProfile;

internal sealed record PreviousUserDetails(
    Guid UserId,
    string UserEmail,
    string UserName,
    string UserRole
);
