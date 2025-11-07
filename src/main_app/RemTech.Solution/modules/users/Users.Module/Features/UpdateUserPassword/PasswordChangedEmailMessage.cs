using Mailing.Moduled.Bus;

namespace Users.Module.Features.UpdateUserPassword;

internal sealed class PasswordChangedEmailMessage(MailingBusPublisher publisher)
{
    public async Task Send(string emailTo, CancellationToken ct = default)
    {
        await publisher.Send(FormMessage(emailTo), ct);
    }

    public MailingBusMessage FormMessage(string emailTo)
    {
        return new MailingBusMessage(
            emailTo,
            "Ваш пароль был изменен.",
            "Изменение пароля RemTech агрегатор спецтехники."
        );
    }
}