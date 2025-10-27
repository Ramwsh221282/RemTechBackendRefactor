using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain;

public static class CleanersDomainDependencyInjection
{
    public static void AddCleanersDomain(this IServiceCollection services)
    {
        services.AddHandlersFromAssembly(typeof(CleanersDomainDependencyInjection).Assembly);
    }
}
