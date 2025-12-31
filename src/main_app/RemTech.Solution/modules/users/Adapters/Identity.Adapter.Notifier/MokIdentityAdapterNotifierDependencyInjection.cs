using Identity.Notifier.Port;

namespace Identity.Adapter.Notifier;

public static class MokIdentityAdapterNotifierDependencyInjection
{
    public static void AddUsersNotifier(this IServiceCollection services)
    {
        services.AddScoped<IUsersNotifier, MokIdentityAdapterNotifier>();
    }
}
