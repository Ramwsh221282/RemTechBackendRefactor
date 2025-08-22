using Mailing.Module.Bus;
using Shared.Infrastructure.Module.Frontend;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminEmailMessage(
    string password,
    Guid confirmationKey,
    FrontendUrl frontendUrl,
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
        string body = $"""
            Вы были зарегистрированы на <a href="{frontendUrl.Read()}">RemTech агрегатор спецтехники</a>
            Вы можете подтвердить почту: <a href="{Generate()}">Подтверждение почты</a>
            Ваш пароль для входа: {password}
            """;
        return new MailingBusMessage(
            emailTo,
            body,
            "Регистрация на RemTech агрегатор спецтехники."
        );
    }

    public string Generate()
    {
        string frontendUrlString = frontendUrl.Read();
        string keyString = confirmationKey.ToString();
        return string.Format(Template, frontendUrlString, keyString);
    }
}
