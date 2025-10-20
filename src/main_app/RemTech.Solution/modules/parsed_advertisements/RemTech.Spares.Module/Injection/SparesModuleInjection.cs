using Microsoft.Extensions.DependencyInjection;
using RemTech.Spares.Module.Features.SinkSpare;

namespace RemTech.Spares.Module.Injection;

public static class SparesModuleInjection
{
    public static void InjectSparesModule(this IServiceCollection services)
    {
        services.AddHostedService<SpareSinkEntrance>();
    }
}
