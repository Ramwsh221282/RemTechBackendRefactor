using Identity.Core.PermissionsModule;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Serilog;

namespace Identity.Logging;

public static class AddSubjectPermissionLogging
{
    internal static AddSubjectPermission AddSubjectPermission(
        AddSubjectPermission origin, 
        ILogger logger) => async args =>
    {
        Result<Subject> result = await origin(args);
        if (result.IsFailure)
        {
            object[] failureProperties = [args.SubjectId, args.PermissionId, false, result.Error.Message];
            
            logger.Error(
                """
                Добавление разрешения:
                Subject ID: {Id},
                Permission ID: {PermissionId},
                Успешен: {Success},
                Ошибка: {Error}
                """,
                failureProperties);
            
            return result.Error;
        }
        
        PermissionSnapshot permissionSnapshot = args.TargetPermission.Value.Snapshot();
        SubjectSnapshot subjectSnapshot = args.TargetSubject.Value.Snapshot();

        object[] successProperties = 
            [
                subjectSnapshot.Id, 
                subjectSnapshot.Email, 
                subjectSnapshot.Login, 
                permissionSnapshot.Id, 
                permissionSnapshot.Name,
                true
            ];
        
        logger.Information(
            """
            Добавление разрешения:
            Subject ID: {Id},
            Subject Email: {Email},
            Subject Login: {Login},
            Permission ID: {PermissionId},
            Permission Name: {PermissionName},
            Успешен: {Success}
            """,
            successProperties);

        return result;
    };

    extension(AddSubjectPermission origin)
    {
        public AddSubjectPermission WithLogging(IServiceProvider sp)
        {
            ILogger logger = sp.Resolve<ILogger>();
            return AddSubjectPermission(origin, logger);
        }
    }
}