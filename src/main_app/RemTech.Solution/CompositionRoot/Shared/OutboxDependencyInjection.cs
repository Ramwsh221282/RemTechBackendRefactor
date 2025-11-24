using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Outbox;

namespace CompositionRoot.Shared;

public static class OutboxDependencyInjection
{
    public static void RegisterOutboxServices(this IServiceCollection services, params string[] schemas)
    {
        services.AddOutboxServices(schemas);
    }
}