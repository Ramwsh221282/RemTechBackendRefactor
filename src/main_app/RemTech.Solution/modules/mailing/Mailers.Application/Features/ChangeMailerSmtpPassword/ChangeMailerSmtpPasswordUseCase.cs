using Mailers.Application.Configs;
using Mailers.Application.Features.Encryptions;
using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.Extensions.Options;

namespace Mailers.Application.Features.ChangeMailerSmtpPassword;

public sealed record ChangeMailerSmtpPasswordUseCase(NpgSqlMailersCommands NpgSql, IOptions<MailersEncryptOptions> Options)
{
    public async Task<Result<Mailer>> Invoke(ChangeMailerSmtpPasswordArgs args)
    {
        Result<SmtpPassword> encrypted = await Options.Encrypted(args.NextPassword);
        if (encrypted.IsFailure) return encrypted.Error;
        return await NpgSql.ExecuteUnderTransaction(async (pg) =>
        {
            Optional<Mailer> mailer = await pg.GetMailer(new QueryMailerArguments(Id: args.Id, WithLock: true), args.Ct);
            if (mailer.NoValue) return NotFound("Почтовый отправитель не существует.");
            Result<Mailer> changed = mailer.Value.WithOtherPassword(encrypted);
            if (changed.IsFailure) return changed.Error;
            Result<Unit> updating = await pg.ExecuteUpdate(changed, args.Ct);
            return updating.Continue(changed);
            
        }, args.Ct);
    }
}