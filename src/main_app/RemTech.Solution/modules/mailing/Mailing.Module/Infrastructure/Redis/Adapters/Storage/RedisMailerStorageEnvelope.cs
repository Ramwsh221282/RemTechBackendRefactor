using Mailing.Module.Infrastructure.Redis.Adapters.SearchCriteria;

namespace Mailing.Module.Infrastructure.Redis.Adapters.Storage;

internal abstract class RedisMailerStorageEnvelope(IMailersStorage<RedisMailerSearchCriteria> storage)
    : IMailersStorage<RedisMailerSearchCriteria>
{
    public Task Add(IMailer postman, CancellationToken ct = default) =>
        storage.Add(postman, ct);

    public Task Remove(IMailer postman, CancellationToken ct = default) =>
        storage.Remove(postman, ct);

    public Task<IMailer> Find(RedisMailerSearchCriteria criteria, CancellationToken ct = default) =>
        storage.Find(criteria, ct);
}