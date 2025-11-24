using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Infrastructure.Outbox;

public static class OutboxDependencyInjection
{
    public static void AddOutboxServices(this IServiceCollection services, params string[] dbSchemas)
    {
        services.AddSingleton<OutboxServicesRegistry>(_ =>
        {
            OutboxServicesRegistry registry = new();
            
            foreach (string schema in dbSchemas)
                registry.AddService(schema);
            
            return registry;
        });
    }
}