using Mailing.Core.Common;

namespace Mailing.Core.Mailers;

public sealed record MailerDomain(Email Email, string Service, string SmtpHost, int CurrentSend,  int SendLimit)
{
    public MailerDomain WithIncreasedSend()
    {
        int nextSend = CurrentSend + 1;
        return this with { CurrentSend = nextSend };
    }
}