namespace WebHostApplication.Modules.identity.Requests;

public sealed record AuthenticateRequest(string Password, string? Email, string? Login);