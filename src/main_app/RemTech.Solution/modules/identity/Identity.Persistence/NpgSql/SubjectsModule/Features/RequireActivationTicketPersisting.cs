using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class RequireActivationTicketPersisting
{
    private static RequireActivationTicket RequireActivationTicket(
        SubjectTicketsStorage tickets,
        SubjectsStorage subjects,
        Optional<NotificationsRegistry> registry,
        RequireActivationTicket origin) => async args =>
    {
        SubjectQueryArgs query = new(Id: args.SubjectId);
        Optional<Subject> subject = await Subject.From(subjects, query, args.Ct);
        RequireActivationTicketArgs withSubject = args with { Target = subject, Registry = registry };
        
        Result<RequireActivationTicketResult> result = await origin(withSubject);
        if (result.IsFailure)
            return result.Error;
        
        Result<Unit> saving = await result.Value.Ticket.SaveTo(tickets, withSubject.Ct);
        return saving.IsFailure ? saving.Error : result.Value;
    };

    private static RequireActivationTicket RequireActivationTicket(
        NpgSqlSession session,
        RequireActivationTicket origin) => async args =>
    {
        CancellationToken ct = args.Ct;
        await session.GetTransaction(ct);
        Result<RequireActivationTicketResult> result = await origin(args);
        
        if (result.IsFailure)
            return result.Error;
        
        return !await session.Commited(ct) 
            ? Error.Application("Не удается сохранить изменения.") 
            : result;
    };
    
    extension(RequireActivationTicket origin)
    {
        public RequireActivationTicket WithPersistence(IServiceProvider provider, Optional<NotificationsRegistry> registry)
        {
            SubjectTicketsStorage tickets = provider.Resolve<SubjectTicketsStorage>();
            SubjectsStorage subjects = provider.Resolve<SubjectsStorage>();
            return RequireActivationTicket(tickets, subjects, registry, origin);
        }

        public RequireActivationTicket WithTransaction(IServiceProvider provider)
        {
            NpgSqlSession session = provider.Resolve<NpgSqlSession>();
            return RequireActivationTicket(session, origin);
        }
    }
}