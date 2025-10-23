namespace Identity.Domain.Users.Events;

public sealed record UserProfileEventArgs(
    string UserLogin,
    string UserEmail,
    string UserPassword,
    bool EmailConfirmed
);
