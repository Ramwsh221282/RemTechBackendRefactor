using System.Net;
using System.Net.Mail;
using Mailing.Module.Contracts;

namespace Mailing.Module.Models;

internal sealed class EmailSender(string name, string email, string password) : IEmailSender
{
    private const int SmtpPort = 587;

    private readonly string _serviceHost = new ServiceHostFromNameResolver(email).Resolve();

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

    public IEmailSender ChangeEmail(string newEmail) => new EmailSender(name, newEmail, password);

    public IEmailSender ChangePassword(string newPassword) =>
        new EmailSender(name, newPassword, password);

    public EmailSenderOutput Print() => new(name, email, password);

    public async Task<bool> Save(IEmailSendersSource source, CancellationToken ct = default) =>
        await source.Save(this, ct);

    public async Task<bool> Update(IEmailSendersSource source, CancellationToken ct = default) =>
        await source.Update(this, ct);

    public async Task<bool> Remove(IEmailSendersSource source, CancellationToken ct = default) =>
        await source.Remove(this, ct);
}
