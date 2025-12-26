using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailersModule;

namespace Mailers.Application.Features.DeleteMailer;

public sealed record DeleteMailerUseCase(NpgSqlMailersCommands Commands)
{
    public async Task<Result<Unit>> Invoke(DeleteMailerArgs args)
    {
        QueryMailerArguments getMailerArgs = new(Id: args.Id, WithLock: true);
        Optional<Mailer> mailer = await Commands.GetMailer(getMailerArgs, args.Ct);
        return mailer.NoValue
            ? NotFound("Почтовый отправитель не найден.")
            : await Commands.ExecuteDelete(mailer.Value, args.Ct);
    }
}