namespace Users.Module.Features.RegisteringUser;

public sealed record RegisterUserRequest(string Email, string Password, string Name);
