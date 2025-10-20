using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Models.Module.Features.AddModelsOnStartup;
using Models.Module.Public;

namespace Models.Module.Injection;

public static class ModelsModuleInjection
{
    public static void InjectModelsModule(this IServiceCollection services)
    {
        services.AddHostedService<CsvModelsSeeding>();
        services.AddSingleton<IModelPublicApi, ModelPublicApi>();
    }
}
