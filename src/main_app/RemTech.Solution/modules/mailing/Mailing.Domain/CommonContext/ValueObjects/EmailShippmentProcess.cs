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
        IEnumerable<OnShippmentProcessFinish>? onFinish = null,
        IEnumerable<OnShippmentProcessFinishAsync>? onFinishAsync = null
    )
    {
        shipper.Subscribe(this);
        shippment.Subscribe(this);
        await SmtpClient.SendMailAsync(MailMessage, ct);
        EmailShippmentResult result = new EmailShippmentResult(shipper, shippment);
        FinishOnShippmentProcesses(onFinish, result);
        await FinishOnShippmentProcessesAsync(onFinishAsync, result);
        return result;
    }

    public void Dispose() => SmtpClient.Dispose();

    private void FinishOnShippmentProcesses(
        IEnumerable<OnShippmentProcessFinish>? onFinish,
        EmailShippmentResult result
    )
    {
        if (onFinish is null || !onFinish.Any())
            return;
        foreach (OnShippmentProcessFinish action in onFinish)
            action(result);
    }

    private async Task FinishOnShippmentProcessesAsync(
        IEnumerable<OnShippmentProcessFinishAsync>? onFinish,
        EmailShippmentResult result
    )
    {
        if (onFinish is null || !onFinish.Any())
            return;
        foreach (OnShippmentProcessFinishAsync action in onFinish)
            await action(result);
    }
}
