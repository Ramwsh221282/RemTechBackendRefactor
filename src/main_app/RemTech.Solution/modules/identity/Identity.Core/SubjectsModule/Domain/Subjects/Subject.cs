using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.ActivationStatus;
using Identity.Core.SubjectsModule.Domain.Credentials;
using Identity.Core.SubjectsModule.Domain.Metadata;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects.Events;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Domain.Subjects;

public sealed record Subject
{
    private SubjectMetadata Metadata { get; init; }
    private SubjectCredentials Credentials { get; init; }
    private SubjectActivationStatus Activation { get; init; }
    private SubjectPermissions Permissions { get; init; }
    private SubjectTicketsDictionary Tickets { get; init; }
    private Optional<NotificationsRegistry> Registry { get; init; }

    public Result<Subject> Activated()
    {
        Result<SubjectActivationStatus> status = Activation.Activate();
        if (status.IsFailure) return status.Error;
        Subject activated = this with { Activation = status };
        return activated;
    }

    public async Task<Result<Unit>> SaveTo(SubjectsStorage storage, CancellationToken ct)
    {
        if (!await IsEmailUnique(storage, ct))
            return Conflict($"Почта: {Credentials.Email} занят.");
        if (!await IsLoginUnique(storage, ct))
            return Conflict($"Логин: {Metadata.Login} занят.");
        return await storage.Insert(this, ct);
    }

    public Result<Subject> Authorize(string inputPassword, VerifyPassword verify)
    {
        if (!verify(Credentials.Password, inputPassword))
            return Conflict("Пароль неверный");
        Registry.ExecuteOnValue(r => r.Record(new SubjectAuthorized(Snapshot())));
        return this;
    }
    
    public async Task<Result<Unit>> UpdateIn(SubjectsStorage storage, CancellationToken ct)
    {
        return await storage.Update(this, ct);
    }

    public async Task<bool> IsEmailUnique(SubjectsStorage storage, CancellationToken ct)
    {
        return await storage.IsEmailUnique(Credentials.Email, ct);
    }

    public async Task<bool> IsLoginUnique(SubjectsStorage storage, CancellationToken ct)
    {
        return await storage.IsLoginUnique(Metadata.Login, ct);
    }
    
    public Result<Subject> Register()
    {
        Result<SubjectActivationStatus> activated = Activation.Activate();
        if (activated.IsFailure) return activated.Error;
        Registry.ExecuteOnValue(reg => reg.Record(new Registered(Snapshot())));
        return this;
    }

    public Result<Subject> AddPermission(SubjectPermission permission)
    {
        Result<SubjectPermissions> @new = Permissions.Add(permission);
        if (@new.IsFailure) return @new.Error;
        Subject withPermission = WithChangedPermissions(@new);
        return withPermission;
    }

    public Result<SubjectTicket> FormTicket(string type)
    {
        SubjectTicket ticket = SubjectTicket.Create(type);
        if (Tickets.Contains(ticket))
            return Conflict($"Учетная запись уже содержит такую заявку.");
        SubjectTicket signed = Metadata.Sign(ticket);
        Registry.ExecuteOnValue(r => r.Record(signed.Raise()));
        return signed;
    }

    public bool HasTicket(SubjectTicket ticket)
    {
        return Tickets.Contains(ticket);
    }
    
    public Result<SubjectTicket> DropTicket(SubjectTicket ticket)
    {
        if (!Tickets.Contains(ticket))
            return Conflict("Заявка у учетной записи не найдена.");
        Tickets.Remove(ticket);
        return ticket;
    }
    
    public Result<Subject> RemovePermission(SubjectPermission permission)
    {
        Result<SubjectPermissions> @new = Permissions.Remove(permission);
        if (@new.IsFailure) return @new.Error;
        Subject withPermission = WithChangedPermissions(@new);
        return withPermission;
    }

    public Result<Subject> ChangePassword(string nextPassword)
    {
        if (AccountNotActivated()) return Conflict("Нельзя изменить пароль пока учетная запись не активирована");
        Result<SubjectCredentials> @new = Credentials.WithOtherPassword(nextPassword);
        if (@new.IsFailure) return @new.Error;
        Subject withOtherPassword = this with { Credentials = @new.Value };
        Registry.ExecuteOnValue(r => r.Record(new PasswordChanged(withOtherPassword.Snapshot())));
        return withOtherPassword;
    }

    public Result<Subject> ChangeEmail(string nextEmail)
    {
        if (AccountNotActivated()) return Conflict("Нельзя изменить почту пока учетная запись не активирована");
        Result<SubjectCredentials> @new = Credentials.WithOtherEmail(nextEmail);
        if (@new.IsFailure) return @new.Error;
        Subject withOtherCredentials = this with { Credentials = @new.Value };
        Registry.ExecuteOnValue(r => r.Record(new EmailChanged(withOtherCredentials.Snapshot())));
        return withOtherCredentials;
    }
    
    public Subject BindRegistry(Optional<NotificationsRegistry> registry)
    {
        return this with { Registry = registry };
    }
    
    public SubjectSnapshot Snapshot()
    {
        return new SubjectSnapshot(
            Metadata.Id,
            Credentials.Email,
            Metadata.Login,
            Credentials.Password,
            Activation.ActivationDate.HasValue ? Activation.ActivationDate.Value : null,
            Permissions.Snapshotted(),
            Tickets.Snapshot());
    }
    
    private bool AccountActivated()
    {
        return Activation.Activated;
    }

    private bool AccountNotActivated() => !AccountActivated();

    private Subject WithUpdatedTickets(SubjectTicketsDictionary ticketsDictionary)
    {
        return this with { Tickets = ticketsDictionary };
    }

    private Subject WithChangedPermissions(SubjectPermissions permissions)
    {
        return this with { Permissions = permissions };
    }
    
    public SubjectPermission Sign(SubjectPermission permission)
    {
        return Metadata.Sign(permission);
    }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTicketsDictionary tickets)
    {
        Metadata = metadata;
        Credentials = credentials;
        Activation = status;
        Permissions = permissions;
        Tickets = tickets;
        Registry = None<NotificationsRegistry>();
    }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTicketsDictionary tickets,
        NotificationsRegistry registry)
    {
        Metadata = metadata;
        Credentials = credentials;
        Activation = status;
        Permissions = permissions;
        Tickets = tickets;
        Registry = Some(registry);
    }
}