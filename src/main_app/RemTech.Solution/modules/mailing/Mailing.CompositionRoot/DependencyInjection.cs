using Mailing.Adapters.Cache;
using Mailing.Adapters.Storage;
using Mailing.CompositionRoot.Postmans;
using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.UseCases.CreatePostman;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.Core.Shared.Primitives.Async;

namespace Mailing.CompositionRoot;

public static class DependencyInjection
{
    public static void AddMailingModule(this IServiceCollection services)
    {
        services.AddCache();
        services.AddStorageAdapter();
        services.AddScoped<CreatePostmanInteractor>();
        services.AddScoped<ComposedAsync<ICreatePostmanUseCase, IPostman>>();
        services.AddSingleton<IPostmansFactory>(_ => new PostmansValidatingFactory(new PostmansFactory()));
    }
}