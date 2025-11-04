using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.UseCases.CreatePostman;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;

namespace Mailing.Adapters.Cache.Postmans.UseCases;

public sealed class CreatePostmanCacheUseCase(
    ComposedAsync<ICreatePostmanUseCase, IPostman> delayedExecution,
    PostmansCache cache,
    ICreatePostmanUseCase useCase) : CreatePostmanUseCaseEnvelope(useCase)
{
    public override Status<IPostman> Create(PostmanConstructionContext context)
    {
        Status<IPostman> result = base.Create(context);
        if (result.IsFailure)
            return result.Error;

        IPostman postman = result.Value;
        delayedExecution.Add(DelayedAction(postman));
        return result;
    }

    private Async<ICreatePostmanUseCase, Status<IPostman>> DelayedAction(
        IPostman postman) =>
        new(async _ =>
        {
            CachedPostman cached = new(cache, postman);
            if (await cached.HasUniqueEmail() == false)
                return ErrorOnNotUnqiqueEmail(cached);
            await cached.Save();
            return cached;
        });

    private Error ErrorOnNotUnqiqueEmail(IPostman postman) =>
        Error.Conflict(
            $"Не удается закешировать информацию почтового сервиса. Email: {postman.Data.Email} занят.");
}