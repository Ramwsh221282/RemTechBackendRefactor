using CompositionRoot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Core.Contracts;
using Tickets.Logging;
using Tickets.Persistence.UseCases;

namespace CompositionRoot.tickets.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class MarkPendingUseCase
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<MarkPending>(sp =>
        {
            MarkPending core = Tickets.Core.UseCases.MarkPendingUseCase.MarkPending;
            MarkPending persisted = core.WithPersistence(sp);
            MarkPending logging = persisted.WithLogging(sp);
            return logging;
        });
    }
}