using Mailing.Module.Domain.Models;
using Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.Storage.Decorators;

internal class MailersStorageEnvelope(IMailersStorage<PgMailerSearchCriteria> origin)
    : IMailersStorage<PgMailerSearchCriteria>
{
    public virtual Task Add(IMailer postman, CancellationToken ct = default) =>
        origin.Add(postman, ct);

    public virtual Task Remove(IMailer postman, CancellationToken ct = default) =>
        origin.Remove(postman, ct);

    public virtual Task<IMailer> Find(PgMailerSearchCriteria criteria, CancellationToken ct = default) =>
        origin.Find(criteria, ct);
}