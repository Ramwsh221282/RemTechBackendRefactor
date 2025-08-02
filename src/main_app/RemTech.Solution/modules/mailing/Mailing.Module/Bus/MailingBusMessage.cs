using Mailing.Module.Contracts;

namespace Mailing.Module.Bus;

public sealed class MailingBusMessage(string to, string body)
{
    public Task Send(IEmailSender sender, CancellationToken ct = default) =>
        sender.FormEmailMessage().Send(to, "Регистрация Rem Tech", body, ct);
}
