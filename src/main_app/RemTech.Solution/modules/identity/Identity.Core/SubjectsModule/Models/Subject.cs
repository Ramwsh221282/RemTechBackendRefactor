using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Core.TicketsModule;
using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record SubjectSnapshot(
    Guid Id,
    string Email,
    string Login,
    string Password,
    DateTime? ActivationDate,
    SubjectPermissionSnapshot[] Permissions);

public sealed record SubjectPermissionSnapshot(
    Guid Id,
    string Name);

public sealed record SubjectTickets(SubjectTicket[] Tickets)
{
    public bool ContainsLastNotActiveWithSameType(SubjectTicket ticket)
    {
        Optional<SubjectTicket> optional = Tickets.TryFind(t => t.Type == ticket.Type);
        return optional.HasValue;
    }

    public SubjectTickets WithoutLastActiveOfType(SubjectTicket ticket)
    {
        return new SubjectTickets(Tickets.Without(ticket, t => t.Type == ticket.Type));
    }

    public SubjectTickets With(SubjectTicket ticket)
    {
        return new SubjectTickets(Tickets.With(ticket));
    }

    public static SubjectTickets Empty() => new([]);
    internal SubjectTickets Copy() => new([..Tickets]);
}

public sealed record SubjectTicket(Guid Id, string Type, bool Active)
{
    internal SubjectTicket(Ticket ticket) : this(ticket.Id, ticket.Type, ticket.Active) { }
}

public sealed record Subject(
    SubjectMetadata MetaData,
    SubjectCredentials Credentials,
    SubjectActivationStatus Activation,
    SubjectPermissions Permissions,
    SubjectTickets Tickets,
    Optional<NotificationsRegistry> EventsRegistry)
{
    public Result<Subject> Registered()
    {
        Result<SubjectActivationStatus> activated = Activation.Activate();
        if (activated.IsFailure) return activated.Error;
        EventsRegistry.ExecuteOnValue(reg => reg.Record(new Registered(Snapshotted())));
        return this;
    }

    public Result<TicketRegistration> RequireAccountActivation()
    {
        if (Activation.Activated) return Conflict("Учетная запись уже активирована.");
        SubjectTickets tickets = Tickets.Copy();
        Ticket activationTicket = Ticket.New(MetaData.Id, TicketType.AccountActivation);
        SubjectTicket subjectTicket = ToSubjectTicket(activationTicket);
        
        if (tickets.ContainsLastNotActiveWithSameType(subjectTicket))
            tickets = tickets.WithoutLastActiveOfType(subjectTicket).With(subjectTicket);
        
        Subject subjectWithNewTickets = WithUpdatedTickets(tickets);
        
        EventsRegistry.ExecuteOnValue(r => 
            r.Record(new CreatedTicket(
                subjectWithNewTickets.Snapshotted(), 
                activationTicket.Snapshotted())));
        
        return new TicketRegistration(subjectWithNewTickets, activationTicket);
    }
    
    public SubjectSnapshot Snapshotted()
    {
        return new SubjectSnapshot(
            MetaData.Id,
            Credentials.Email,
            MetaData.Login,
            Credentials.Password,
            Activation.ActivationDate.HasValue ? Activation.ActivationDate.Value : null,
            Permissions.Snapshotted());
    }

    public Result<Subject> WithOtherPassword(string nextPassword, HashPassword hash)
    {
        SubjectCredentialsConstructionContext ctx = new(Credentials.Email, nextPassword);
        Result<SubjectCredentials> @new = SubjectCredentials.Create(ctx);
        if (@new.IsFailure) return @new.Error;
        string credentialsPassword = @new.Value.Password;
        string hashedPassword = hash(credentialsPassword);
        SubjectCredentials withHashedPassword = @new.Value with { Password = hashedPassword };
        Subject withChangedCredentials = this with { Credentials = withHashedPassword };
        EventsRegistry.ExecuteOnValue(r => r.Record(new PasswordChanged(withChangedCredentials.Snapshotted())));
        return withChangedCredentials;
    }

    public Result<Subject> WithOtherEmail(string nextEmail)
    {
        if (!Activation.Activated)
            return Conflict("Нельзя изменить почту пока учетная запись не активирована");
        SubjectCredentialsConstructionContext ctx = new(nextEmail, Credentials.Password);
        Result<SubjectCredentials> @new = SubjectCredentials.Create(ctx);
        if (@new.IsFailure) return @new.Error;
        Subject withOtherCredentials = this with { Credentials = @new.Value };
        EventsRegistry.ExecuteOnValue(r => r.Record(new EmailChanged(withOtherCredentials.Snapshotted())));
        return withOtherCredentials;
    }
    
    private static SubjectTicket ToSubjectTicket(Ticket ticket)
    {
        return new SubjectTicket(ticket);
    }

    private Subject WithUpdatedTickets(SubjectTickets tickets)
    {
        return this with { Tickets = tickets };
    }

    internal Subject AttachRegistry(NotificationsRegistry registry)
    {
        return this with { EventsRegistry = Some(registry) };
    }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions)
        : this(
            metadata, 
            credentials, 
            status, 
            permissions, 
            SubjectTickets.Empty(), None<NotificationsRegistry>()) { }
}