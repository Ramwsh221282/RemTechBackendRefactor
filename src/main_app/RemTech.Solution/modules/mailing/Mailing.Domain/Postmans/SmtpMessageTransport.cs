using System.Net.Mail;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.Postmans;

public sealed class SmtpMessageTransport : IMessageTransport
{
    private const int Port = 587;

    public void Send(MessageDeliveryContext context, AsyncDelayedExecutionVeil veil)
    {
        SmtpClient client = new SmtpClient(context.Service, Port);
        MailAddress from = new MailAddress(context.From);
        MailAddress to = new MailAddress(context.To);

        MailMessage message = new MailMessage(from, to)
        {
            Body = context.Body,
            Subject = context.Subject
        };

        veil.Enqueue(Invoke(client, message));
    }

    private static Func<Task<Status<Unit>>> Invoke(SmtpClient client, MailMessage message) =>
        async () =>
        {
            try
            {
                await client.SendMailAsync(message);
                return Unit.Value;
            }
            finally
            {
                client.Dispose();
            }
        };
}