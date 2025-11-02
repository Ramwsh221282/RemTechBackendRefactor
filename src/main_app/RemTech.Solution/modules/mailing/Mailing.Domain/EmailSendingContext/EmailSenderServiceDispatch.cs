using Mailing.Domain.EmailSendingContext.ValueObjects;
using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext;

public sealed class EmailSenderServiceDispatch(Status<EmailString> email)
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    private readonly Status<NotEmptyString> _dispatchedServiceName = Error.None();

    public Status<NotEmptyString> Dispatch() =>
        from email_value in email
        from yandex_dispatch in DispatchedEmailService(email_value, "yandex.ru", "smtp.yandex.ru")
        from mail_ru_dispatch in DispatchedEmailService(email_value, "mail.ru", "smtp.mail.ru")
        from gmail_com_dispatch in DispatchedEmailService(email_value, "gmail.com", "smtp.gmail.com")
        select DispatchedEmailService(yandex_dispatch, mail_ru_dispatch, gmail_com_dispatch);

    private Status<NotEmptyString> DispatchedEmailService(params Status<string>[] dispatches)
    {
        return dispatches.All(d => d.IsFailure)
            ? Error.Validation(
                $"Не удается разрешить внешнего провайдера отправки по почте.")
            : NotEmptyString.New(dispatches.Single(el => el.IsSuccess));
    }

    private Status<string> DispatchedEmailService(EmailString email, string domain, string compatibleService)
    {
        string emailString = email._value;
        return emailString switch
        {
            _ when emailString.EndsWith(domain, Comparison) => compatibleService,
            _ => Error.Validation(
                $"Не удается разрешить внешнего провайдера отправки по почте для почты: {emailString}")
        };
    }

    public EmailSenderServiceDispatch(string emailString) : this(EmailString.Create(emailString))
    {
    }
}