using Mailing.Moduled.Contracts;

namespace Mailing.Moduled.Bus;

public sealed record MailingBusMessage(string To, string Body, string Subject)
{
    public Task Send(IEmailSender sender, CancellationToken ct = default) =>
        sender.FormEmailMessage().Send(To, Subject, Body, ct);
}