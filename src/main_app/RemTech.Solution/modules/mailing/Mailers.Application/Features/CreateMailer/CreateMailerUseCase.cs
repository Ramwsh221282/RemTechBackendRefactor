using Mailers.Application.Configs;
using Mailers.Application.Features.Encryptions;
using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.Extensions.Options;

namespace Mailers.Application.Features.CreateMailer;

public sealed record CreateMailerUseCase(NpgSqlMailersCommands NpgSql, IOptions<MailersEncryptOptions> Options)
{
    public async Task<Result<Mailer>> Invoke(CreateMailerArgs args)
    {
        Result<Mailer> mailer = Mailer.Construct(args.Email, args.SmtpPassword, Optional<Guid>.None());
        if (mailer.IsFailure) return mailer.Error;
        
        Result<Mailer> encrypted = await mailer.Value.WithEncryptedPassword(Options);
        if (encrypted.IsFailure) return encrypted.Error;
        
        if (!await NpgSql.IsEmailUnique(encrypted.Value.Metadata.Email, args.Ct))
            return Conflict($"Почтовый отправитель с почтой: {encrypted.Value.Metadata.Email.Value} уже существует.");
        
        Result<Unit> inserting = await NpgSql.ExecuteInsert(encrypted, args.Ct);
        return inserting.IsFailure ? inserting.Error : mailer.Value;
    }
}