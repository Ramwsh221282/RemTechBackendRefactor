using Mailing.Adapters.Cache.Postmans;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.Adapters.Cache;

public static class DependencyInjection
{
    public static void AddCache(this IServiceCollection services)
    {
        services.AddScoped<PostmansCache>();
    }
}