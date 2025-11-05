using Mailing.Domain.General;
using Mailing.Domain.Postmans.Storing;
using RemTech.Core.Shared.Primitives.Async;
using StackExchange.Redis;

namespace Mailing.Adapters.Cache.Postmans;

public sealed class RedisPostmans(IDatabase database, DelayedAsyncAction action) : IPostmansStorage
{
    private JsonPostman _jsonPostman = new(database);

    public void Save(int sendLimit, int currentSend) =>
        _jsonPostman.Save(sendLimit, currentSend);

    public void Save(Guid id, string email, string password) =>
        _jsonPostman.Save(id, email, password);

    public void Save(Action<IPostmanMetadataStorage, IPostmanStatisticsStorage> saveDelegate)
    {
        saveDelegate(_jsonPostman, _jsonPostman);
        JsonPostman json = new(database);
        action.Enqueue(new AsyncAction(async () =>
        {
            if (!await json.HasUniqueEmail())
                throw new ConflictOperationException("Postman с таким Email уже существует в кеше.");
            await json.Save();
        }));
    }
}