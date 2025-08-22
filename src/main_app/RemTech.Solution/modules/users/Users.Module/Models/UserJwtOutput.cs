namespace Users.Module.Models;

internal sealed record UserJwtOutput(
    Guid UserId,
    string Name,
    string Password,
    string Email,
    string Role,
    string Token,
    string RefreshToken
);
