using Notifications.Core.Mailers;

namespace WebHostApplication.Modules.notifications;

public sealed record MailerResponse(
    Guid Id,
    string Email,
    string SmtpHost
)
{
    public static MailerResponse ConvertFrom(Mailer mailer) =>
        new(mailer.Id.Value, 
            mailer.Credentials.Email, 
            mailer.Credentials.SmtpHost);
}