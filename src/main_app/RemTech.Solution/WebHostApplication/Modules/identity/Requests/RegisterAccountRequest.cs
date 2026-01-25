namespace WebHostApplication.Modules.identity.Requests;

public sealed record RegisterAccountRequest(string Email, string Login, string Password);
