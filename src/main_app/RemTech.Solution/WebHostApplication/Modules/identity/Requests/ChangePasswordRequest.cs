namespace WebHostApplication.Modules.identity.Requests;

public record ChangePasswordRequest(string NewPassword, string CurrentPassword);