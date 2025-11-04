using Mailing.Adapters.Cache.Postmans;
using Mailing.Adapters.Cache.Postmans.UseCases;
using Mailing.Adapters.Storage.Postmans.UseCases;
using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.UseCases.CreatePostman;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.CompositionRoot.Postmans;

public sealed class CreatePostmanGateway
{
    public Task<Status<IPostman>> Invoke(
        ComposedAsync<ICreatePostmanUseCase, IPostman> execution,
        Serilog.ILogger logger,
        PostmansCache cache,
        IPostmansFactory factory,
        PostgresDatabase database,
        CancellationToken ct) =>
        execution.ExecuteAsync(new CreatePostmanUseCase(factory)
            .With(f => new CreatePostmanDbUseCase(execution, database, ct, f))
            .With(f => new CreatePostmanCacheUseCase(execution, cache, f))
            .With(f => new CreatePostmanLoggingUseCase(logger, f)));
}