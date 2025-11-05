using Mailing.Adapters.Cache;
using Mailing.Adapters.Storage;
using Mailing.CompositionRoot.Postmans;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.CompositionRoot;

public static class DependencyInjection
{
    public static void AddMailingModule(this IServiceCollection services)
    {
        services.AddCache();
        services.AddStorageAdapter();
        services.AddScoped<CreatePostmanInteractor>();
    }
}