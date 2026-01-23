using Notifications.Core.Mailers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

public sealed record MailerResponse(Guid Id, string Email, string SmtpHost)
{
    public static MailerResponse Create(Mailer mailer) =>
        new(mailer.Id.Value, mailer.Credentials.Email, mailer.Credentials.SmtpHost);
}
