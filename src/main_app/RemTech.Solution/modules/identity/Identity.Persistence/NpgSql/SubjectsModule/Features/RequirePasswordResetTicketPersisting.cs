using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class RequirePasswordResetTicketPersisting
{
    private static RequirePasswordResetTicket WithPersisting(
        RequirePasswordResetTicket origin,
        SubjectsStorage subjects,
        SubjectTicketsStorage tickets,
        Optional<NotificationsRegistry> registry) => async args =>
    {
        Optional<Subject> subject = await subjects.GetById(args.SubjectId, args.Ct);
        Result<SubjectTicket> ticket = await origin(args with { Target = subject, Registry = registry });
        if (ticket.IsFailure) return ticket.Error;
        Result<Unit> saving = await tickets.Insert(ticket, args.Ct);
        return saving.IsFailure? saving.Error : ticket.Value;
    };

    private static RequirePasswordResetTicket
        WithTransaction(RequirePasswordResetTicket origin, NpgSqlSession session) =>
        async args =>
        {
            await session.GetTransaction(args.Ct);
            Result<SubjectTicket> ticket = await origin(args);
            if (ticket.IsFailure) return ticket.Error;
            if (!await session.Commited(args.Ct)) 
                return Error.Application("Не удается зафиксировать изменения.");
            return ticket.Value;
        };

    extension(RequirePasswordResetTicket origin)
    {
        public RequirePasswordResetTicket WithPersisting(IServiceProvider sp, Optional<NotificationsRegistry> registry)
        {
            SubjectsStorage subjects = sp.Resolve<SubjectsStorage>();
            SubjectTicketsStorage tickets = sp.Resolve<SubjectTicketsStorage>();
            return WithPersisting(origin, subjects, tickets, registry);
        }

        public RequirePasswordResetTicket WithTransaction(IServiceProvider sp)
        {
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            return WithTransaction(origin, session);
        }
    }
}