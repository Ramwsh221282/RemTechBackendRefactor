using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.ActivationStatus;
using Identity.Core.SubjectsModule.Domain.Credentials;
using Identity.Core.SubjectsModule.Domain.Metadata;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Domain.Subjects;

public sealed record Subject
{
    internal SubjectMetadata Metadata { get; init; }
    internal SubjectCredentials Credentials { get; init; }
    internal SubjectActivationStatus Activation { get; init; }
    internal SubjectPermissions Permissions { get; init; }
    internal SubjectTickets Tickets { get; init; }
    internal Optional<NotificationsRegistry> Registry { get; init; }

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
    
    public Subject BindRegistry(NotificationsRegistry registry)
    {
        return this with { Registry = Some(registry) };
    }
    
    public SubjectSnapshot Snapshot()
    {
        return new SubjectSnapshot(
            Metadata.Id,
            Credentials.Email,
            Metadata.Login,
            Credentials.Password,
            Activation.ActivationDate.HasValue ? Activation.ActivationDate.Value : null,
            Permissions.Snapshotted());
    }
    
    private bool AccountActivated()
    {
        return Activation.Activated;
    }

    private bool AccountNotActivated() => !AccountActivated();

    private Subject WithUpdatedTickets(SubjectTickets tickets)
    {
        return this with { Tickets = tickets };
    }

    private Subject WithChangedPermissions(SubjectPermissions permissions)
    {
        return this with { Permissions = permissions };
    }

    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTickets tickets)
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
        SubjectTickets tickets,
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