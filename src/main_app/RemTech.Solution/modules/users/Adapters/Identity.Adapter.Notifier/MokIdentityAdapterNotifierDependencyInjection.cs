using Identity.Notifier.Port;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Adapter.Notifier;

public static class MokIdentityAdapterNotifierDependencyInjection
{
    public static void AddUsersNotifier(this IServiceCollection services)
    {
        services.AddScoped<IUsersNotifier, MokIdentityAdapterNotifier>();
    }
}
