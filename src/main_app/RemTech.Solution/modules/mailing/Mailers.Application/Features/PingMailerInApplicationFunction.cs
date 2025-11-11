using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record PingMailerInApplicationFunctionArgument(Guid Id, string To, NpgSqlSession Session)
    : IFunctionArgument;

public sealed class
    PingMailerInApplicationFunction : IAsyncFunction<PingMailerInApplicationFunctionArgument, Result<Mailer>>
{
    private readonly IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>> _ping;
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;
    private readonly IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>> _insert;
    private readonly IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> _sendEmail;

    public PingMailerInApplicationFunction(
        IFunction<PingMailerSenderFunctionArgument, Result<MailerSending>> ping,
        IFunction<CreateEmailFunctionArgument, Result<Email>> createEmail,
        IAsyncFunction<InsertMailerSendingFunctionArguments, Result<MailerSending>> insert,
        IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>> sendEmail
    )
    {
        _ping = ping;
        _createEmail = createEmail;
        _insert = insert;
        _sendEmail = sendEmail;
    }


    public async Task<Result<Mailer>> Invoke(PingMailerInApplicationFunctionArgument argument, CancellationToken ct)
    {
        CreateEmailFunctionArgument createEmailArg = new(argument.To);
        Result<Email> email = _createEmail.Invoke(createEmailArg);
        if (email.IsFailure) return email.Error;

        NpgSqlSession session = argument.Session;
        await session.GetTransaction(ct);
        Optional<Mailer> mailer = await new QueryMailerArguments(Id: argument.Id).Get(session, ct, true);
        if (mailer.NoValue) return NotFound("Почтовый отправитель не найден.");

        Result<MailerSending> pinging = _ping.Invoke(new PingMailerSenderFunctionArgument(mailer.Value, argument.To));
        if (pinging.IsFailure) return pinging.Error;

        Result<MailerSending> inserting =
            await _insert.Invoke(new InsertMailerSendingFunctionArguments(pinging, session), ct);
        if (inserting.IsFailure) return inserting.Error;

        SendEmailByMimeKitFunctionArgument sendEmailArg = new(
            inserting.Value.Mailer,
            inserting.Value.Message.DeliveryInfo.To,
            inserting.Value.Message.Content.Subject,
            inserting.Value.Message.Content.Body);

        Result<Unit> sendingEmail = await _sendEmail.Invoke(sendEmailArg, ct);
        if (sendingEmail.IsFailure) return sendingEmail.Error;

        if (!await session.Commited(ct))
            return Error.Application("Не удается зафиксировать создание нового отправленного сообщения.");

        return pinging.Value.Mailer;
    }
}