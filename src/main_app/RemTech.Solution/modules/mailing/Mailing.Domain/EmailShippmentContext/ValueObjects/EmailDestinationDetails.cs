using System.Net.Mail;
using Mailing.Domain.CommonContext.ValueObjects;

namespace Mailing.Domain.EmailShippmentContext.ValueObjects;

public sealed record EmailDestinationDetails(EmailAddress From, EmailAddress To)
{
    public void Subscribe(EmailShippmentProcess process)
    {
        process.MailMessage.From = new MailAddress(From.Value);
        process.MailMessage.To.Add(new MailAddress(To.Value));
    }
}
