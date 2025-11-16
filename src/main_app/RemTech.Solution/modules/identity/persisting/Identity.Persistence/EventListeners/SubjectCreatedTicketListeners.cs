using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects.Events;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Core.TicketsModule;
using Identity.Core.TicketsModule.Contracts;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.EventListeners;

public static class SubjectCreatedTicketListeners
{
    private static AsyncNotificationHandle<SubjectCreatedTicket> TicketMustBeCreated(TicketsStorage tickets) => async (@event, ct) =>
    {
        Guid id = @event.Id;
        Guid creatorId = @event.CreatorId;
        string type = @event.Type;
        Ticket ticket = Ticket.Create(id, creatorId, type);
        Result<Unit> saving = await ticket.SaveTo(tickets, ct);
        return saving.IsSuccess ? Unit.Value : saving.Error;
    };
    
    private static async Task<Result<T>> ProcessWithNotificationsHandle<T>(
        Func<Task<Result<T>>> origin, 
        NotificationsRegistry registry, 
        CancellationToken ct)
    {
        Result<T> result = await origin();
        if (result.IsFailure) return result.Error;
        Result<Unit> handling = await registry.ProcessNotifications(ct);
        return handling.IsSuccess ? result.Value : handling.Error;
    }
    
    private static RequirePasswordResetTicket WithTicketsListening(
        RequirePasswordResetTicket origin,
        NotificationsRegistry registry
    ) => async args => 
        await ProcessWithNotificationsHandle(async () => await origin(args), registry, args.Ct);
    
    private static RequireActivationTicket WithTicketsListening(
        NotificationsRegistry registry,
        RequireActivationTicket origin) => async args =>
        await ProcessWithNotificationsHandle(async () => await origin(args), registry, args.Ct);

    extension(RequireActivationTicket origin)
    {
        public RequireActivationTicket WithTicketsListening(IServiceProvider sp, NotificationsRegistry registry)
        {
            TicketsStorage tickets = sp.Resolve<TicketsStorage>();
            AsyncNotificationHandle<SubjectCreatedTicket> handle = TicketMustBeCreated(tickets);
            registry.AddNotificationHandler(handle);
            RequireActivationTicket decorated = SubjectCreatedTicketListeners.WithTicketsListening(registry, origin);
            return decorated;
        }
    }

    extension(RequirePasswordResetTicket origin)
    {
        public RequirePasswordResetTicket WithTicketsListening(IServiceProvider sp, NotificationsRegistry registry)
        {
            TicketsStorage tickets = sp.Resolve<TicketsStorage>();
            AsyncNotificationHandle<SubjectCreatedTicket> handle = TicketMustBeCreated(tickets);
            registry.AddNotificationHandler(handle);
            return SubjectCreatedTicketListeners.WithTicketsListening(origin, registry);
        }
    }
}