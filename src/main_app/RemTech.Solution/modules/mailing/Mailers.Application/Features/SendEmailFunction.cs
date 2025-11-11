using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record SendEmailFunctionArgument(Mailer Mailer, string Email, string Subject, string Body)
    : IFunctionArgument;

public sealed class SendEmailFunction : IAsyncFunction<SendEmailFunctionArgument, Result<MailerSending>>
{
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;
    private readonly IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> _sendEmailByMimeKit;

    public SendEmailFunction(
        IFunction<CreateEmailFunctionArgument, Result<Email>> function,
        IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> sendEmailByMimeKit
    )
    {
        _createEmail = function;
        _sendEmailByMimeKit = sendEmailByMimeKit;
    }

    public async Task<Result<MailerSending>> Invoke(SendEmailFunctionArgument argument, CancellationToken ct)
    {
        Result<Email> email = _createEmail.Invoke(new CreateEmailFunctionArgument(argument.Email));
        Mailer mailer = argument.Mailer;
        string subject = argument.Subject;
        string body = argument.Body;
        if (email.IsFailure) return email.Error;
        Result<MailerSending> sending = mailer.SendEmail(email, subject, body);
        if (sending.IsFailure) return sending.Error;
        SendEmailByMimeKitFunctionArgument sendByMimeKit = new(mailer, email, subject, body);
        Result<Unit> sendingbyMime = await _sendEmailByMimeKit.Invoke(sendByMimeKit, ct);
        return sendingbyMime.IsFailure ? sendingbyMime.Error : sending;
    }
}