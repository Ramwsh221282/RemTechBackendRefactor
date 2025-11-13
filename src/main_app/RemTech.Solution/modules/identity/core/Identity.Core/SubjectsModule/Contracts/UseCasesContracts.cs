using Identity.Core.PermissionsModule;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Translations;

namespace Identity.Core.SubjectsModule.Contracts;

public sealed record RegisterSubjectUseCaseArgs(string Login, string Email, string Password, CancellationToken Ct);
public delegate Task<Result<Subject>> RegisterSubject(RegisterSubjectUseCaseArgs args);

public sealed record AddSubjectPermissionArgs(
    Guid PermissionId, 
    Guid SubjectId, 
    Optional<Subject> TargetSubject, 
    Optional<Permission> TargetPermission, 
    CancellationToken ct);

public delegate Task<Result<Subject>> AddSubjectPermission(AddSubjectPermissionArgs args);

public static class SubjectUseCases
{
    public static AddSubjectPermission AddSubjectPermission() => (args) =>
    {
        if (args.TargetSubject.NoValue) return Task.FromResult<Result<Subject>>(NotFound("Учетная запись не найдена."));
        if (args.TargetPermission.NoValue) return Task.FromResult<Result<Subject>>(NotFound("Разрешение не найдено."));

        Permission permission = args.TargetPermission.Value;
        Subject subject = args.TargetSubject.Value;
        
        SubjectPermission subjectPermission = SubjectToPermissionTranslations.PermissionToSubjectPermission(permission);
        Result<Subject> withPermission = subject.AddPermission(subjectPermission);
        
        return withPermission.IsFailure 
            ? Task.FromResult<Result<Subject>>(withPermission.Error) 
            : Task.FromResult(withPermission);
    };
}