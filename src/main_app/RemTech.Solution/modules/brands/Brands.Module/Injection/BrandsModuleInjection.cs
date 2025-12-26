using Brands.Module.Public.GetBrand;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;

namespace Brands.Module.Injection;

public static class BrandsModuleInjection
{
    public static void InjectBrandsModule(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(BrandsModuleInjection).Assembly);
        services.AddScoped<IGetBrandApi, GetBrandApi>();
    }
}
