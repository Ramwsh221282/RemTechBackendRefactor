namespace Users.Module.Features.CreatingNewAccount;

public sealed record RegisterUserRequest(string Email, string Password, string Name);
