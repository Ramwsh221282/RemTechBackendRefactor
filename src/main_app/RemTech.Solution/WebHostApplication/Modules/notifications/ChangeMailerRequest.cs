namespace WebHostApplication.Modules.notifications;

public sealed record ChangeMailerRequest(string SmtpPassword, string Email);
