using Mailing.Tests.CleanWriteTests.Models;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Redis;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.Redis;

public sealed class WritePostmanInRedis
    : IWritePostmanInfrastructureCommand
{
    private readonly RedisPostman _postman;
    private readonly EventHandler<EventArgs> _handler;

    public WritePostmanInRedis(RedisCache redis, DelayedUnitStatus container)
    {
        _postman = new RedisPostman(redis);
        _handler += async (_, _) => await InsertPostmanInCache(container);
    }

    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        metadata.Write(_postman);
        statistics.Write(_postman);
        _handler?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertPostmanInCache(DelayedUnitStatus container)
    {
        try
        {
            await _postman.Save();
            container.Accept(Unit.Value);
        }
        catch (Exception)
        {
            container.Accept(Error.Internal("Не удается сохранить Postman в кеш. Ошибка на стороне приложения."));
        }
    }
}