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
    internal static AsyncNotificationHandle<SubjectCreatedTicket> TicketMustBeCreated(TicketsStorage tickets) => async (@event, ct) =>
    {
        Guid id = @event.Id;
        Guid creatorId = @event.CreatorId;
        string type = @event.Type;
        Ticket ticket = Ticket.Create(id, creatorId, type);
        Result<Unit> saving = await ticket.SaveTo(tickets, ct);
        return saving.IsSuccess ? Unit.Value : saving.Error;
    };

    internal static RequireActivationTicket WithTicketsListening(
        NotificationsRegistry registry,
        RequireActivationTicket origin) => async args =>
    {
        Result<SubjectTicket> result = await origin(args);
        if (result.IsFailure)
            return result.Error;
        
        Result<Unit> handling = await registry.ProcessNotifications(args.Ct);
        return handling.IsFailure ? handling.Error : result;
    };

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
}