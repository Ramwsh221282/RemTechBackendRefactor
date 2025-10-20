using Brands.Module.Features.AddBrandsOnStartup;
using Brands.Module.Public;
using Microsoft.Extensions.DependencyInjection;

namespace Brands.Module.Injection;

public static class BrandsModuleInjection
{
    public static void InjectBrandsModule(this IServiceCollection services)
    {
        services.AddHostedService<SeedingBrandsOnStartup>();
        services.AddSingleton<IBrandsPublicApi, BrandsPublicApi>();
    }
}
