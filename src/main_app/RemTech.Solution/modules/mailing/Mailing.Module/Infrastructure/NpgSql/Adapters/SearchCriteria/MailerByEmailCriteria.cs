using Dapper;
using Mailing.Module.Domain.Models;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;

internal sealed class MailerByEmailCriteria(string email) : PgMailerSearchCriteria
{
    private const string Sql = "SELECT * FROM mailing_module.postmans WHERE email = @email;";

    public override Task<IMailer> Find(CancellationToken ct = default) =>
        Handle(new CommandDefinition(Sql, new { email }, cancellationToken: ct), ct);
}