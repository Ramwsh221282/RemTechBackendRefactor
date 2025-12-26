namespace Identity.WebApi.Requests;

public sealed record UserRegistrationRequest(string Login, string Email, string Password);
