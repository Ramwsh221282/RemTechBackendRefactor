namespace Identity.Domain.Users.Events;

public sealed record IdentityUserProfileEventArgs(
    string UserLogin,
    string UserEmail,
    string UserPassword,
    bool EmailConfirmed
);