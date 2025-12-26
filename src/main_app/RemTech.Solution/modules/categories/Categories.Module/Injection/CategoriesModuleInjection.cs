using Categories.Module.Public;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Injection;

public static class CategoriesModuleInjection
{
    public static void InjectCategoriesModule(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(CategoriesModuleInjection).Assembly);
        services.AddSingleton<IGetCategoryApi, GetCategoryApi>();
    }
}
