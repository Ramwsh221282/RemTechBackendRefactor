namespace WebHostApplication.Modules.notifications;

public sealed record AddMailerRequest(string SmtpPassword, string Email);