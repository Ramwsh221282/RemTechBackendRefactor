namespace Mailing.Core.Mailers;

public sealed class MailerUpdateShell(string? newEmail, string? newSmtpPassword)
{
    private readonly MailerDomainUpdateShell _domainUpdate = new(newEmail, newSmtpPassword);

    public Mailer Update(Mailer mailer)
    {
        Mailer withUpdatedDomain = _domainUpdate.Update(mailer);
        return withUpdatedDomain;
    }
}