using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule.Features;

public static class AddSubjectPermissionPersisting
{
    private static AddSubjectPermission AddSubjectPermission(
        AddSubjectPermission origin,
        SubjectsStorage subjects,
        PermissionsStorage permissions) => async args =>
    {
        CancellationToken ct = args.Ct;
        Optional<Subject> subject = await subjects.GetById(args.SubjectId, ct, true);
        Optional<Permission> permission = await permissions.GetById(args.PermissionId, ct);

        AddSubjectPermissionArgs withEntities = args with
        {
            TargetSubject = subject, 
            TargetPermission = permission
        };
        
        Result<Subject> result = await origin(withEntities);
        if (result.IsFailure) return result.Error;
        
        SubjectPermission subjectPermission = permission.Value.ToSubjectPermission();
        Result<Unit> permissionInserting = await subjects.InsertPermission(result, subjectPermission, ct);
        return permissionInserting.IsFailure ? permissionInserting.Error : result;
    };

    private static AddSubjectPermission AddSubjectPermissionTransactional
        (NpgSqlSession session, AddSubjectPermission origin) => async args =>
    {
        CancellationToken ct = args.Ct;
        await session.GetTransaction(ct);
        Result<Subject> result = await origin(args);
        if (result.IsFailure) 
            return result.Error;
        
        if (!await session.Commited(ct))
            return Error.Application("Не удается зафиксировать изменения.");

        return result.Value;
    };
    
    extension(AddSubjectPermission origin)
    {
        public AddSubjectPermission WithPersisting(IServiceProvider sp)
        {
            SubjectsStorage subjects = sp.Resolve<SubjectsStorage>();
            PermissionsStorage permissions = sp.Resolve<PermissionsStorage>();
            return AddSubjectPermission(origin, subjects, permissions);
        }

        public AddSubjectPermission WithTransaction(IServiceProvider sp)
        {
            NpgSqlSession session = sp.Resolve<NpgSqlSession>();
            return AddSubjectPermissionTransactional(session, origin);
        }
    }
}