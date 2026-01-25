namespace WebHostApplication.Modules.identity.Requests;

public record ResetPasswordRequest(string? Login, string? Email);
