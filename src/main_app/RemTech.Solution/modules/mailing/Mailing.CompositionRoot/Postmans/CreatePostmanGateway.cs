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

public sealed class CreatePostmanGateway(Serilog.ILogger logger, PostmansCache cache, PostgresDatabase database)
{
    public async Task<Status<IPostman>> Invoke(PostmanConstructionContext context, CancellationToken ct)
    {
        ComposedAsync<ICreatePostmanUseCase, IPostman> execution = new();

        ICreatePostmanUseCase component =
            new CreatePostmanUseCase(new PostmansFactory().With(f => new PostmansValidatingFactory(f)))
                .With(f => new CreatePostmanDbUseCase(execution, database, ct, f))
                .With(f => new CreatePostmanCacheUseCase(execution, cache, f))
                .With(f => new CreatePostmanLoggingUseCase(logger, f));

        Status<IPostman> postman = component.Create(context);
        return postman.IsFailure ? postman.Error : await execution.ExecuteAsync(component);
    }
}