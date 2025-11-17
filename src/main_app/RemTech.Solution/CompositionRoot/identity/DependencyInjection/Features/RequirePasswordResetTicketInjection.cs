using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Logging;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
public static class RequirePasswordResetTicketInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<RequirePasswordResetTicket>(sp =>
        {
            NotificationsRegistry registry = new();
            RequirePasswordResetTicket core = SubjectUseCases.RequirePasswordResetTicket;
            RequirePasswordResetTicket persisted = core.WithPersisting(sp, registry);
            // RequirePasswordResetTicket ticketReacted = persisted.WithTicketsListening(sp, registry);
            RequirePasswordResetTicket transactional = persisted.WithTransaction(sp);
            RequirePasswordResetTicket logging = transactional.WithLogging(sp);
            return logging;
        });
    }
}