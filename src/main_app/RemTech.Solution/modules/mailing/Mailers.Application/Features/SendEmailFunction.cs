using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record SendEmailFunctionArgument(Mailer Mailer, string Email, string Subject, string Body) : IFunctionArgument;

public class SendEmailFunction : IFunction<SendEmailFunctionArgument, Result<MailerSending>>
{
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;

    public SendEmailFunction(IFunction<CreateEmailFunctionArgument, Result<Email>> function) =>
        _createEmail = function;
    
    public Result<MailerSending> Invoke(SendEmailFunctionArgument argument)
    {
        var email = _createEmail.Invoke(new CreateEmailFunctionArgument(argument.Email));
        var mailer = argument.Mailer;
        var subject = argument.Subject;
        var body = argument.Body;
        if (email.IsFailure) return email.Error;
        return mailer.SendEmail(email, subject, body);
    }
}