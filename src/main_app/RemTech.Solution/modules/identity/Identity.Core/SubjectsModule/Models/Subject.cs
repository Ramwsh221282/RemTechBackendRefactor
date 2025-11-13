using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Core.TicketsModule;
using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubjectSnapshot(
    Guid Id,
    string Email,
    string Login,
    string Password,
    DateTime? ActivationDate,
    IdentitySubjectPermissionSnapshot[] Permissions);

public sealed record IdentitySubjectPermissionSnapshot(
    Guid Id,
    string Name);

public sealed record SubjectTickets
{
    private readonly SubjectTicket[] _tickets;
    
    internal SubjectTickets()
    {
        _tickets = [];
    }
    
    internal SubjectTickets(SubjectTicket[] tickets)
    {
        _tickets = tickets;
    }

    public bool ContainsLastNotActiveWithSameType(SubjectTicket ticket)
    {
        Optional<SubjectTicket> optional = _tickets.TryFind(t => t.Type == ticket.Type);
        return optional.HasValue;
    }

    public SubjectTickets WithoutLastActiveOfType(SubjectTicket ticket)
    {
        return new SubjectTickets(_tickets.Without(ticket, t => t.Type == ticket.Type));
    }

    public SubjectTickets With(SubjectTicket ticket)
    {
        return new SubjectTickets(_tickets.With(ticket));
    }

    public static SubjectTickets Empty() => new([]);

    internal SubjectTickets Copy() => new([.._tickets]);
}

public sealed record SubjectTicket(Guid Id, string Type, bool Active)
{
    internal SubjectTicket(Ticket ticket) : this(ticket.Id, ticket.Type, ticket.Active)
    {
        
    }
}

public sealed record Subject
{
    private SubjectMetadata _metaData { get; init; }
    private SubjectCredentials _credentials { get; init; }
    private SubjectActivationStatus _activation { get; init; }
    private SubjectPermissions _permissions { get; init; }
    private SubjectTickets _tickets { get; init; }
    private Optional<SubjectEventsRegistry> _eventsRegistry { get; init; }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectEventsRegistry? eventsRegistry = null)
    {
        _metaData = metadata;
        _credentials = credentials;
        _activation = status;
        _permissions = permissions;
        _tickets = SubjectTickets.Empty();
        _eventsRegistry = FromNullable(eventsRegistry);
    }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTickets tickets,
        SubjectEventsRegistry? eventsRegistry = null)
    {
        _metaData = metadata;
        _credentials = credentials;
        _activation = status;
        _permissions = permissions;
        _tickets = tickets;
        _eventsRegistry = FromNullable(eventsRegistry);
    }

    internal Subject(Subject origin, SubjectEventsRegistry registry) :
        this(origin._metaData, origin._credentials, origin._activation, origin._permissions)
    {
        _eventsRegistry = Some(registry);
    }
    
    public Result<Subject> Registered()
    {
        Result<SubjectActivationStatus> activated = _activation.Activate();
        if (activated.IsFailure) return activated.Error;
        _eventsRegistry.ExecuteOnValue(reg => reg.Record(new IdentitySubjectRegisteredNotification(Snapshotted())));
        return this;
    }

    public Result<TicketRegistration> RequireAccountActivation()
    {
        if (_activation.Activated) return Conflict("Учетная запись уже активирована.");
        SubjectTickets tickets = _tickets.Copy();
        Ticket activationTicket = Ticket.New(_metaData.Id, TicketType.AccountActivation);
        SubjectTicket subjectTicket = ToSubjectTicket(activationTicket);
        if (tickets.ContainsLastNotActiveWithSameType(subjectTicket))
            tickets = tickets.WithoutLastActiveOfType(subjectTicket).With(subjectTicket);
        Subject subjectWithNewTickets = WithUpdatedTickets(tickets);
        return new TicketRegistration(subjectWithNewTickets, activationTicket);
    }
    
    public IdentitySubjectSnapshot Snapshotted()
    {
        return new IdentitySubjectSnapshot(
            _metaData.Id,
            _credentials.Email,
            _metaData.Login,
            _credentials.Password,
            _activation.ActivationDate.HasValue ? _activation.ActivationDate.Value : null,
            _permissions.Snapshotted());
    }

    private SubjectTicket ToSubjectTicket(Ticket ticket)
    {
        return new SubjectTicket(ticket);
    }

    private Subject WithUpdatedTickets(SubjectTickets tickets)
    {
        return this with { _tickets = tickets };
    }
}