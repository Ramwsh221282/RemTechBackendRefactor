using CompositionRoot.Shared;
using Identity.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Outbox;

[DependencyInjectionClass]
internal static class DependencyInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddIdentityOutboxProcessor();
    }
}