using Categories.Module.Features.AddCategoriesOnStartup;
using Categories.Module.Public;
using Microsoft.Extensions.DependencyInjection;

namespace Categories.Module.Injection;

public static class CategoriesModuleInjection
{
    public static void InjectCategoriesModule(this IServiceCollection services)
    {
        services.AddHostedService<SeedingCategoriesOnStartup>();
        services.AddSingleton<ICategoryPublicApi, CategoryPublicApi>();
    }
}
