using System.Net;
using System.Net.Mail;
using Mailing.Moduled.Contracts;

namespace Mailing.Moduled.Models;

internal sealed class EmailSender(string email, string password) : IEmailSender
{
    private const int SmtpPort = 587;

    private readonly string _name = new ServiceNameResolver(email).Resolved();

    private readonly string _serviceHost = new ServiceHostFromNameResolver(email).Resolved();

    public IEmailMessage FormEmailMessage()
    {
        return new EmailMessage(
            new SmtpClient(_serviceHost, SmtpPort)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true,
            },
            new MailMessage { From = new MailAddress(email) }
        );
    }

    public EmailSenderOutput Print() => new(_name, email, password);

    public async Task<bool> Save(IEmailSendersSource source, CancellationToken ct = default) =>
        await source.Save(this, ct);

    public async Task<bool> Remove(IEmailSendersSource source, CancellationToken ct = default) =>
        await source.Remove(this, ct);
}