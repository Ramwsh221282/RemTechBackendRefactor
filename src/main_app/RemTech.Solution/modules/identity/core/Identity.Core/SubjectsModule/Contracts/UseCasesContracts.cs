using Identity.Core.PermissionsModule;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Contracts;

public sealed record RegisterSubjectUseCaseArgs(string Login, string Email, string Password, CancellationToken Ct);
public delegate Task<Result<Subject>> RegisterSubject(RegisterSubjectUseCaseArgs args);

public sealed record AddSubjectPermissionArgs(
    Guid PermissionId, 
    Guid SubjectId, 
    Optional<Subject> TargetSubject, 
    Optional<Permission> TargetPermission, 
    CancellationToken Ct);
public delegate Task<Result<Subject>> AddSubjectPermission(AddSubjectPermissionArgs args);

public sealed record ChangeEmailArgs(Guid Id, string NextEmail, Optional<Subject> Target, CancellationToken Ct);
public delegate Task<Result<Subject>> ChangeEmail(ChangeEmailArgs args);

public sealed record ChangePasswordArgs(Guid Id, string NextPassword, Optional<Subject> Target, CancellationToken Ct);
public delegate Task<Result<Subject>> ChangePassword(ChangePasswordArgs args);

public sealed record RequireActivationTicketArgs(Guid SubjectId, Optional<Subject> Target, Optional<NotificationsRegistry> Registry, CancellationToken Ct);
public delegate Task<Result<SubjectTicket>> RequireActivationTicket(RequireActivationTicketArgs args);

public static class SubjectUseCases
{
    public static RequireActivationTicket RequireActivationTicket => args =>
    {
        if (args.Target.NoValue) 
            return Task.FromResult<Result<SubjectTicket>>(NotFound("Учетная запись не найдена."));
        Subject subject = args.Target.Value.BindRegistry(args.Registry);
        Result<SubjectTicket> ticket = subject.FormTicket("user.account.activation");
        return ticket.IsFailure 
            ? Task.FromResult<Result<SubjectTicket>>(ticket.Error) 
            : Task.FromResult(ticket);
    };
    
    public static AddSubjectPermission AddSubjectPermission => args =>
    {
        if (args.TargetSubject.NoValue) 
            return Task.FromResult<Result<Subject>>(NotFound("Учетная запись не найдена."));
        
        if (args.TargetPermission.NoValue) 
            return Task.FromResult<Result<Subject>>(NotFound("Разрешение не найдено."));

        Permission permission = args.TargetPermission.Value;
        Subject subject = args.TargetSubject.Value;
        
        SubjectPermission subjectPermission = permission.ToSubjectPermission();
        Result<Subject> withPermission = subject.AddPermission(subjectPermission);
        
        return withPermission.IsFailure 
            ? Task.FromResult<Result<Subject>>(withPermission.Error) 
            : Task.FromResult(withPermission);
    };
    
    public static ChangeEmail ChangeEmail => args =>
    {
        if (args.Target.NoValue) return 
            Task.FromResult<Result<Subject>>(NotFound("Учетная запись не найдена."));
        
        Subject subject = args.Target.Value;
        Result<Subject> withOtherEmail = subject.ChangeEmail(args.NextEmail);
        return withOtherEmail.IsSuccess 
            ? Task.FromResult(withOtherEmail) 
            : Task.FromResult<Result<Subject>>(withOtherEmail.Error);
    };

    public static ChangePassword ChangePassword => args =>
    {
        if (args.Target.NoValue)
            return Task.FromResult<Result<Subject>>(NotFound("Учетная запись не найдена."));

        Result<Subject> withOtherPassword = args
            .Target
            .Value
            .ChangePassword(args.NextPassword);

        return withOtherPassword.IsFailure 
            ? Task.FromResult<Result<Subject>>(withOtherPassword.Error) 
            : Task.FromResult(withOtherPassword);
    };
    
    public static RegisterSubject RegisterSubject => args =>
    {
        string email = args.Email;
        string login = args.Login;
        string password = args.Password;
        Result<Subject> subject = Subject.Create(email, login, password);
        return Task.FromResult(subject);
    };
}