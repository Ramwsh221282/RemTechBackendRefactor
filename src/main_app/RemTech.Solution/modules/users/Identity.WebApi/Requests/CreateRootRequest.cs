namespace Identity.WebApi.Requests;

public sealed record CreateRootRequest(string Login, string Email, string Password);
