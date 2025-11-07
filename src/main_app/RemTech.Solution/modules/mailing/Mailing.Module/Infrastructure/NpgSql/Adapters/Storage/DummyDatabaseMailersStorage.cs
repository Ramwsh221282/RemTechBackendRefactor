namespace Mailing.Module.Infrastructure.NpgSql.Adapters.Storage;

internal sealed class DummyDatabaseMailersStorage<TSearchCriteria> : IMailersStorage<TSearchCriteria>
    where TSearchCriteria : IMailersSearchCriteria
{
    public Task Add(IMailer postman, CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task Remove(IMailer postman, CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task<IMailer> Find(TSearchCriteria criteria, CancellationToken ct = default) =>
        Task.FromResult<IMailer>(new EmptyMailer());
}