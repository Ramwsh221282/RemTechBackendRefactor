namespace Identity.WebApi.Requests;

public sealed record AuthRequest(string Password, string? Login, string? Email);
