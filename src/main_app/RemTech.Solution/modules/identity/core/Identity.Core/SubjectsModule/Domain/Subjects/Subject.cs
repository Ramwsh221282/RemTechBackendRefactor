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
    internal SubjectMetadata _metadata { get; init; }
    internal SubjectCredentials Credentials { get; init; }
    internal SubjectActivationStatus _activation { get; init; }
    internal SubjectPermissions _permissions { get; init; }
    internal SubjectTickets _tickets { get; init; }
    internal Optional<NotificationsRegistry> _registry { get; init; }
    
    public Result<Subject> Register()
    {
        Result<SubjectActivationStatus> activated = _activation.Activate();
        if (activated.IsFailure) return activated.Error;
        _registry.ExecuteOnValue(reg => reg.Record(new Registered(Snapshot())));
        return this;
    }

    public Result<Subject> AddPermission(SubjectPermission permission)
    {
        Result<SubjectPermissions> @new = _permissions.Add(permission);
        if (@new.IsFailure) return @new.Error;
        Subject withPermission = WithChangedPermissions(@new);
        return withPermission;
    }

    public Result<Subject> RemovePermission(SubjectPermission permission)
    {
        Result<SubjectPermissions> @new = _permissions.Remove(permission);
        if (@new.IsFailure) return @new.Error;
        Subject withPermission = WithChangedPermissions(@new);
        return withPermission;
    }

    public Result<Subject> ChangePassword(string nextPassword, HashPassword hash)
    {
        if (AccountNotActivated()) return Conflict("Нельзя изменить пароль пока учетная запись не активирована");
        Result<SubjectCredentials> @new = Credentials.WithOtherPassword(nextPassword, hash);
        if (@new.IsFailure) return @new.Error;
        Subject withOtherPassword = this with { Credentials = @new.Value };
        _registry.ExecuteOnValue(r => r.Record(new PasswordChanged(withOtherPassword.Snapshot())));
        return withOtherPassword;
    }

    public Result<Subject> ChangeEmail(string nextEmail)
    {
        if (AccountNotActivated()) return Conflict("Нельзя изменить почту пока учетная запись не активирована");
        Result<SubjectCredentials> @new = Credentials.WithOtherEmail(nextEmail);
        if (@new.IsFailure) return @new.Error;
        Subject withOtherCredentials = this with { Credentials = @new.Value };
        _registry.ExecuteOnValue(r => r.Record(new EmailChanged(withOtherCredentials.Snapshot())));
        return withOtherCredentials;
    }
    
    public Subject BindRegistry(NotificationsRegistry registry)
    {
        return this with { _registry = Some(registry) };
    }
    
    public SubjectSnapshot Snapshot()
    {
        return new SubjectSnapshot(
            _metadata.Id,
            Credentials.Email,
            _metadata.Login,
            Credentials.Password,
            _activation.ActivationDate.HasValue ? _activation.ActivationDate.Value : null,
            _permissions.Snapshotted());
    }
    
    private bool AccountActivated()
    {
        return _activation.Activated;
    }

    private bool AccountNotActivated() => !AccountActivated();

    private Subject WithUpdatedTickets(SubjectTickets tickets)
    {
        return this with { _tickets = tickets };
    }

    private Subject WithChangedPermissions(SubjectPermissions permissions)
    {
        return this with { _permissions = permissions };
    }

    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTickets tickets)
    {
        _metadata = metadata;
        Credentials = credentials;
        _activation = status;
        _permissions = permissions;
        _tickets = tickets;
        _registry = None<NotificationsRegistry>();
    }
    
    internal Subject(
        SubjectMetadata metadata,
        SubjectCredentials credentials,
        SubjectActivationStatus status,
        SubjectPermissions permissions,
        SubjectTickets tickets,
        NotificationsRegistry registry)
    {
        _metadata = metadata;
        Credentials = credentials;
        _activation = status;
        _permissions = permissions;
        _tickets = tickets;
        _registry = Some(registry);
    }
}