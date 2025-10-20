using Mailing.Module.Bus;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminEmailMessage(
    string password,
    Guid confirmationKey,
    IOptions<FrontendOptions> frontendUrl,
    MailingBusPublisher publisher
)
{
    private const string Template = "{0}/email-confirmation?confirmationKey={1}";
    private readonly string _frontendString = frontendUrl.Value.FrontendUrl;

    public async Task Send(string emailTo, CancellationToken ct = default) =>
        await publisher.Send(FormMessage(emailTo), ct);

    private MailingBusMessage FormMessage(string emailTo)
    {
        string body = $"""
            Вы были зарегистрированы на <a href="{_frontendString}">RemTech агрегатор спецтехники</a>
            Вы можете подтвердить почту: <a href="{Generate()}">Подтверждение почты</a>
            Ваш пароль для входа: {password}
            """;

        return new MailingBusMessage(
            emailTo,
            body,
            "Регистрация на RemTech агрегатор спецтехники."
        );
    }

    private string Generate()
    {
        string keyString = confirmationKey.ToString();
        return string.Format(Template, _frontendString, keyString);
    }
}
