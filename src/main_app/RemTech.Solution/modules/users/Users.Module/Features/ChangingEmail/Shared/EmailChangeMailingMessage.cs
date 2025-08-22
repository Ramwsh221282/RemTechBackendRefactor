using Mailing.Module.Bus;
using Shared.Infrastructure.Module.Frontend;

namespace Users.Module.Features.ChangingEmail.Shared;

internal sealed class EmailChangeMailingMessage(
    FrontendUrl frontendUrl,
    Guid confirmationKey,
    MailingBusPublisher publisher
)
{
    private const string Template = "{0}/email-confirmation?confirmationKey={1}";

    public async Task Send(string emailTo, CancellationToken ct = default)
    {
        await publisher.Send(FormMessage(emailTo), ct);
    }

    public MailingBusMessage FormMessage(string emailTo)
    {
        return new MailingBusMessage(
            emailTo,
            $"""
            Ваша почта была изменена, однако Вам нужно подтвердить изменение.
            Для этого необходимо перейти по ссылке:
            <a href="{Generate()}">Подтверждение почты</a>
            """,
            "Изменение почты RemTech агрегатор спецтехники."
        );
    }

    public string Generate()
    {
        string frontendUrlString = frontendUrl.Read();
        string keyString = confirmationKey.ToString();
        return string.Format(Template, frontendUrlString, keyString);
    }
}
