using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record PingMailerSenderFunctionArgument(Mailer Mailer, string Email) : IFunctionArgument;

public sealed class PingMailerSenderFunction : IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>>
{
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;
    private readonly IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> _sendByMimeKit;

    public PingMailerSenderFunction(
        IFunction<CreateEmailFunctionArgument, Result<Email>> function,
        IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> sendByMimeKit
    )
    {
        _createEmail = function;
        _sendByMimeKit = sendByMimeKit;
    }

    public Result<MailerSending> Invoke(PingMailerSenderFunctionArgument argument)
    {
        Result<Email> email = _createEmail.Invoke(new CreateEmailFunctionArgument(argument.Email));
        if (email.IsFailure) return email.Error;
        Mailer mailer = argument.Mailer;
        return mailer.SendEmail(email, "Тестовая отправка", "Тестовая отправка сообщения.");
    }
}