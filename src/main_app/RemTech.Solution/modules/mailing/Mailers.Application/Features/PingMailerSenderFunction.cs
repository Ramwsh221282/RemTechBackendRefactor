using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record PingMailerSenderFunctionArgument(Mailer Mailer, string Email) : IFunctionArgument;

public sealed class PingMailerSenderFunction : IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>>
{
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;

    public PingMailerSenderFunction(IFunction<CreateEmailFunctionArgument, Result<Email>> function) =>
        _createEmail = function;
    
    public Result<MailerSending> Invoke(PingMailerSenderFunctionArgument argument)
    {
        var email = _createEmail.Invoke(new CreateEmailFunctionArgument(argument.Email));
        if (email.IsFailure) return email.Error;
        var mailer = argument.Mailer;
        return mailer.SendEmail(email, "Тестовая отправка", "Тестовая отправка сообщения.");
    }
}