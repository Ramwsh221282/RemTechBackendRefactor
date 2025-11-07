using Mailing.Module.Infrastructure.Redis.Adapters.SearchCriteria;
using Mailing.Module.Infrastructure.Redis.Models;
using Shared.Infrastructure.Module.Redis;

namespace Mailing.Module.Infrastructure.Redis.Adapters.Storage;

internal sealed class RedisMailersStorage(RedisCache cache, TimeSpan lifeTime)
    : IMailersStorage<RedisMailerSearchCriteria>
{
    public async Task Add(IMailer postman, CancellationToken ct = default) =>
        await new RedisMailer(cache, postman, lifeTime).Save();

    public async Task Remove(IMailer postman, CancellationToken ct = default) =>
        await new RedisMailer(cache, postman, lifeTime).Delete();

    public async Task<IMailer> Find(RedisMailerSearchCriteria criteria, CancellationToken ct = default)
    {
        criteria.AttachRedis(cache.Database);
        return await criteria.Find(ct);
    }
}