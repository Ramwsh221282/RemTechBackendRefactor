namespace Users.Module.Models.Features.CreatingNewAccount;

public sealed record RegisterUserRequest(string Email, string Password, string Name);
