using Mailers.Application.Configs;
using Mailers.Application.Features.MailerSmtpProviding;
using Mailers.Core.EmailsModule;
using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.Extensions.Options;

namespace Mailers.Application.Features.SendEmailMessage;

public sealed record SendEmailMessageUseCase(NpgSqlMailersCommands Commands, IOptions<MailersEncryptOptions> Encrypt)
{
    public async Task<Result<Mailer>> Invoke(SendEmailMessageArgs args)
    {
        Result<Email> email = Email.Construct(args.To);
        if (email.IsFailure) return email.Error;
        
        MailerDeliveryArgs delivery = new(email, args.Subject, args.Body);
        
        return await Commands.ExecuteUnderTransaction<Result<Mailer>>(async (pg) =>
        {
            QueryMailerArguments queryMailerArgs = new(Id: args.Id);
            
            Optional<Mailer> mailer = await pg.GetMailer(queryMailerArgs, args.Ct);
            if (mailer.NoValue) return NotFound("Почтовый отправитель не найден.");
            
            Result<MailerSending> sent = await mailer.Value.SendMessage(Encrypt, delivery, args.Ct);
            if (sent.IsFailure) return sent.Error;
            
            Result<Unit> insertingSent = await pg.InsertMailerSending(sent, args.Ct);
            if (insertingSent.IsFailure) return insertingSent.Error;
            
            Result<Unit> updating = await pg.ExecuteUpdate(sent.Value.Mailer, args.Ct);
            if (updating.IsFailure) return updating.Error;
            
            return sent.Value.Mailer;
        }, args.Ct);
    }
}