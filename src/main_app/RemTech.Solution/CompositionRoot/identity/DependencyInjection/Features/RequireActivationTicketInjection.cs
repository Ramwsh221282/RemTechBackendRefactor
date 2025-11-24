using CompositionRoot.Shared;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Persistence.NpgSql.SubjectsModule.Features;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Functional.Extensions;

namespace CompositionRoot.identity.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class RequireActivationTicketInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<RequireActivationTicket>(sp =>
        {
            NotificationsRegistry registry = new();
            RequireActivationTicket core = SubjectUseCases.RequireActivationTicket;
            RequireActivationTicket persisted = core.WithPersistence(sp, Optional.Some(registry));
            RequireActivationTicket outboxListener = persisted.WithOutboxListener(sp);
            RequireActivationTicket transactional = outboxListener.WithTransaction(sp);
            RequireActivationTicket logging = transactional.WithLogging(sp);
            return logging;
        });
    }
}