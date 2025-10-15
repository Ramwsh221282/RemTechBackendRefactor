using Mailing.Domain.CommonContext.ValueObjects;

namespace Mailing.Domain.EmailShippmentContext.ValueObjects;

public sealed record EmailMessageDetails
{
    public string Subject { get; }
    public string Body { get; }

    private EmailMessageDetails(string subject, string body)
    {
        Subject = subject;
        Body = body;
    }

    public void Subscribe(EmailShippmentProcess process)
    {
        process.MailMessage.Subject = Subject;
        process.MailMessage.Body = Body;
    }
}
