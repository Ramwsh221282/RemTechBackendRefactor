using Mailing.Module.Bus;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.ChangingEmail;
using Users.Module.Features.ChangingEmail.Shared;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Features.UserPasswordRecovering.Core;
using Users.Module.Features.UserPasswordRecovering.Exceptions;

namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal sealed class WhenEmailNotConfirmedRecoveringMethod : IUserRecoveringMethod
{
    private const string Template = "{0}/password-reset-confirm?confirmationKey={1}";
    private readonly EmailConfirmationKeyGeneration _emailConfirmationKeyGeneration;
    private readonly PasswordRecoveringCache _passwordRecoveringCache;
    private readonly UserToRecover _user;
    private readonly FrontendUrl _frontendUrl;
    private readonly MailingBusPublisher _publisher;

    public WhenEmailNotConfirmedRecoveringMethod(
        ConnectionMultiplexer multiplexer,
        UserToRecover user,
        FrontendUrl frontendUrl,
        MailingBusPublisher publisher
    )
    {
        _passwordRecoveringCache = new PasswordRecoveringCache(multiplexer);
        _user = user;
        _emailConfirmationKeyGeneration = new EmailConfirmationKeyGeneration(
            new ConfirmationEmailsCache(multiplexer)
        );
        _frontendUrl = frontendUrl;
        _publisher = publisher;
    }

    public async Task<PasswordResetMessageDetails> Invoke()
    {
        Guid resetPasswordKey = await _passwordRecoveringCache.GenerateRecoveringKey(_user);
        Guid confirmEmailKey = await GenerateEmailConfirmationKey();
        MailingBusMessage message = FormMessage(resetPasswordKey, confirmEmailKey);
        await _publisher.Send(message);
        return PasswordResetMessageDetails.Standard();
    }

    private async Task<Guid> GenerateEmailConfirmationKey()
    {
        _user.Print(out Guid id, out _, out _);
        Guid emailConfirmKey = await _emailConfirmationKeyGeneration.Generate(id);
        return emailConfirmKey;
    }

    private MailingBusMessage FormMessage(Guid resetPasswordKey, Guid confirmEmailKey)
    {
        _user.Print(out _, out string email, out _);
        return new MailingBusMessage(
            email,
            $"""
            Была подана заявка на сброс пароля.
            Ваша почта не была подтверждена, но она была указана при регистрации.

            1. Перед тем как восстановить пароль, Вам необходимо подтвердить вашу почту.
            Для подтверждения почты необходимо перейти по ссылке:
            <a href="{GenerateEmailConfirmationUrl(confirmEmailKey)}">Подтверждение почты</a>

            2. После подтверждения почты, Вы можете сбросить пароль.
            Для сброса пароля необходимо перейти по ссылке: 
            <a href="{GeneratePasswordResetUrl(resetPasswordKey)}">Сброс пароля</a>
            """,
            "Инструкция по сбросу пароля RemTech аггрегатор."
        );
    }

    private string GenerateEmailConfirmationUrl(Guid emailConfirmKey)
    {
        return new EmailConfirmationMailingMessage(
            _frontendUrl,
            emailConfirmKey,
            _publisher
        ).Generate();
    }

    private string GeneratePasswordResetUrl(Guid passwordResetKey)
    {
        string frontendUrl = _frontendUrl.Read();
        string keyString = passwordResetKey.ToString();
        return string.Format(Template, frontendUrl, keyString);
    }
}
