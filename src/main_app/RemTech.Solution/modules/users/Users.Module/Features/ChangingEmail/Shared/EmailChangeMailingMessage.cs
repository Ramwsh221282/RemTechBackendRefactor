using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Users.Module.Features.ChangingEmail.Shared;

internal sealed class EmailChangeMailingMessage(
    IOptions<FrontendOptions> frontendUrl,
    Guid confirmationKey,
    MailingBusPublisher publisher
)
{
    private const string Template = "{0}/email-confirmation?confirmationKey={1}";

    public async Task Send(string emailTo, CancellationToken ct = default)
    {
        await publisher.Send(FormMessage(emailTo), ct);
    }

    private MailingBusMessage FormMessage(string emailTo)
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

    private string Generate()
    {
        string frontendUrlString = frontendUrl.Value.FrontendUrl;
        string keyString = confirmationKey.ToString();
        return string.Format(Template, frontendUrlString, keyString);
    }
}