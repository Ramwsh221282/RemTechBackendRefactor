namespace Mailers.Core.MailersContext;

public readonly struct MailerSmtpServiceName
{
    public readonly string Name;

    public MailerSmtpServiceName(string name)
    {
        Name = name;
    }
}