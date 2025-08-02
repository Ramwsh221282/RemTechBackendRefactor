using System.Net.Mail;
using Mailing.Module.Contracts;

namespace Mailing.Module.Models;

internal sealed class EmailMessage(SmtpClient client, MailMessage message) : IEmailMessage
{
    private bool _sended;

    public async Task Send(string to, string subject, string body, CancellationToken ct = default)
    {
        if (_sended)
            throw new InvalidOperationException("Email has been already send.");
        using (client)
        {
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            await client.SendMailAsync(message, ct);
            _sended = true;
        }
    }
}
