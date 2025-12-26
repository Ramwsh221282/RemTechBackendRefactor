using StackExchange.Redis;
using Users.Module.Features.UserPasswordRecovering.Core;

namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal sealed class WhenEmailConfirmedRecoveringMethod : IUserRecoveringMethod
{
    private const string Template = "{0}/password-reset-confirm?confirmationKey={1}";
    private readonly PasswordRecoveringCache _passwordRecoveringCache;
    private readonly UserToRecover _user;
    private readonly FrontendUrl _frontendUrl;
    private readonly MailingBusPublisher _publisher;

    public WhenEmailConfirmedRecoveringMethod(
        ConnectionMultiplexer multiplexer,
        UserToRecover user,
        FrontendUrl frontendUrl,
        MailingBusPublisher publisher
    )
    {
        _passwordRecoveringCache = new PasswordRecoveringCache(multiplexer);
        _user = user;
        _frontendUrl = frontendUrl;
        _publisher = publisher;
    }

    public async Task<PasswordResetMessageDetails> Invoke()
    {
        Guid key = await _passwordRecoveringCache.GenerateRecoveringKey(_user);
        MailingBusMessage message = FormMessage(key);
        await _publisher.Send(message);
        return PasswordResetMessageDetails.Standard();
    }

    private MailingBusMessage FormMessage(Guid resetPasswordKey)
    {
        _user.Print(out _, out string email, out _);
        return new MailingBusMessage(
            email,
            $"""
             Была подана заявка на сброс пароля.
             Для сброса пароля необходимо перейти по ссылке: 
             <a href="{GeneratePasswordResetUrl(resetPasswordKey)}">Сброс пароля</a>
             """,
            "Инструкция по сбросу пароля RemTech аггрегатор."
        );
    }

    private string GeneratePasswordResetUrl(Guid passwordResetKey)
    {
        string frontendUrl = _frontendUrl.Read();
        string keyString = passwordResetKey.ToString();
        return string.Format(Template, frontendUrl, keyString);
    }
}