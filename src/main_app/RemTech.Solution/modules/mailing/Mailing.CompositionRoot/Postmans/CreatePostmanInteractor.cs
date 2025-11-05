using Mailing.Adapters.Cache.Postmans;
using Mailing.Adapters.Storage.Postmans.Storage;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.Factories.Metadata;
using Mailing.Domain.Postmans.Factories.Statistics;
using RemTech.Core.Shared.Decorating;
using RemTech.Core.Shared.Primitives.Async;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;

namespace Mailing.CompositionRoot.Postmans;

public sealed class CreatePostmanInteractor(
    PostgresDatabase database,
    RedisCache cache,
    Serilog.ILogger logger)
{
    public async Task Invoke(Guid id, string email, string password, CancellationToken ct)
    {
        DelayedAsyncAction action = new();
        new ComposedPostmanStorages()
            .Add(new NpgSqlPostmansStorage(database, action, ct))
            .Add(new RedisPostmans(cache.Database, action))
            .Save(new PostmansFactory(
                new PostmanStatisticsFactory().With(f => new PostmanValidOnlyStatisticsFactory(f))
                    .With(f => new PostmanLoggingStatisticsFactory(logger, f)),
                new PostmanMetadataFactory().With(f => new PostmanValidOnlyMetadataFactory(f))
                    .With(f => new PostmanLoggingMetadataFactory(logger, f))
            ).Construct(id, email, password));
        await action.Execute();
    }
}