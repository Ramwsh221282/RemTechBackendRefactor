using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Translations;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Features;

public static class AddSubjectPermissionPersistingUseCase
{
    internal static AddSubjectPermission AddSubjectPermission(
        AddSubjectPermission origin,
        SubjectsStorage subjects,
        PermissionsStorage permissions) => async args =>
    {
        Guid subjectId = args.SubjectId;
        Guid permissionId = args.PermissionId;
        CancellationToken ct = args.Ct;
        
        SubjectQueryArgs subjectQuery = new(Id: subjectId, WithLock: true);
        PermissionsQueryArgs permissionQuery = new(Id: permissionId);

        Optional<Subject> subject = await subjects.Find(subjectQuery, ct);
        Optional<Permission> permission = await permissions.Find(permissionQuery, ct);

        AddSubjectPermissionArgs withEntities = args with
        {
            TargetSubject = subject, 
            TargetPermission = permission
        };
        
        Result<Subject> result = await origin(withEntities);
        if (result.IsFailure) return result.Error;

        SubjectPermission subjectPermission = BoundedContextConverter.PermissionToSubjectPermission(permission.Value);
        Result<Unit> permissionInserting = await subjects.InsertPermission(result, subjectPermission, ct);
        return permissionInserting.IsFailure ? permissionInserting.Error : result;
    };
    
    extension(AddSubjectPermission origin)
    {
        public AddSubjectPermission WithPersisting(IServiceProvider sp)
        {
            SubjectsStorage subjects = sp.Resolve<SubjectsStorage>();
            PermissionsStorage permissions = sp.Resolve<PermissionsStorage>();
            return AddSubjectPermission(origin, subjects, permissions);
        }
    }
}