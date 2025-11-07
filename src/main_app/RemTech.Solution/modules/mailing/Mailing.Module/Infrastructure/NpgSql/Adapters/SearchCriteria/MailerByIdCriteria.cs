using Dapper;
using Mailing.Module.Domain.Models;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;

internal sealed class MailerByIdCriteria(Guid id) : PgMailerSearchCriteria
{
    private const string Sql = "SELECT * FROM mailing_module.postmans WHERE id = @id;";

    public override Task<IMailer> Find(CancellationToken ct = default) =>
        Handle(new CommandDefinition(Sql, new { id }, cancellationToken: ct), ct);
}