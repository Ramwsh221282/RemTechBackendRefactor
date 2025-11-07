using StackExchange.Redis;

namespace Mailing.Module.Infrastructure.Redis.Adapters.SearchCriteria;

internal abstract class RedisMailerSearchCriteria : IMailersSearchCriteria
{
    protected IDatabase? _database;

    public void AttachRedis(IDatabase database) => _database = database;

    public abstract Task<IMailer> Find(CancellationToken ct = default);
}