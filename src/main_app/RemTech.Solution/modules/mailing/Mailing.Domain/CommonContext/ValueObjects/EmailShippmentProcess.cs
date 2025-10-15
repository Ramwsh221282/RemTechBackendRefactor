using System.Net.Mail;
using Mailing.Domain.EmailShipperContext;
using Mailing.Domain.EmailShippmentContext;

namespace Mailing.Domain.CommonContext.ValueObjects;

public sealed record EmailShippmentProcess : IDisposable
{
    private const int SmtpPort = 587;
    public SmtpClient SmtpClient { get; }
    public MailMessage MailMessage { get; }

    public EmailShippmentProcess(SmtpClient client, MailMessage mailMessage)
    {
        SmtpClient = client;
        client.Port = SmtpPort;
        MailMessage = mailMessage;
    }

    public async Task<EmailShippmentResult> SendAsync(
        EmailShipper shipper,
        EmailShippment shippment,
        CancellationToken ct = default,
        IEnumerable<IEmailShippmentCallback>? callbacks = null
    )
    {
        shipper.Subscribe(this);
        shippment.Subscribe(this);
        await SmtpClient.SendMailAsync(MailMessage, ct);
        EmailShippmentResult result = new EmailShippmentResult(shipper, shippment);
        await ProcessCallbacks(callbacks, result, ct);
        return result;
    }

    public void Dispose() => SmtpClient.Dispose();

    private async Task ProcessCallbacks(
        IEnumerable<IEmailShippmentCallback>? callbacks,
        EmailShippmentResult result,
        CancellationToken ct = default
    )
    {
        if (callbacks is null || !callbacks.Any())
            return;

        foreach (IEmailShippmentCallback callback in callbacks)
        {
            switch (callback)
            {
                case IEmailShippmentNonAsyncCallback nonAsync:
                    nonAsync.Invoke(result);
                    break;
                case IEmailShippmentAsyncCallback asyncCallback:
                    await asyncCallback.Invoke(result, ct);
                    break;
            }
        }
    }
}
