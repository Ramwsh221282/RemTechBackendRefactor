using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record IdentitySubjectPermissionNotificationArg(Guid Id, string Name);

public sealed record IdentitySubjectRegisteredNotification(
    Guid Id, 
    string Email, 
    string Login, 
    string Password,
    IReadOnlyList<IdentitySubjectPermissionNotificationArg> Permissions) : IdentitySubjectNotification
{
    public IdentitySubjectRegisteredNotification(
        IdentitySubjectMetadata metadata,
        IdentitySubjectCredentials credentials,
        IdentitySubjectPermissions permissions
    )
        : this(metadata.Id, credentials.Email, metadata.Login, credentials.Password, [])
    {
        Permissions = FromPermissions(permissions);
    }

    private static IReadOnlyList<IdentitySubjectPermissionNotificationArg> FromPermissions(
        IdentitySubjectPermissions permissions)
    {
        List<IdentitySubjectPermissionNotificationArg> args = [];
        permissions.ForEach((p) => args.Add(new IdentitySubjectPermissionNotificationArg(p.Id, p.Name)));
        return args;
    }
}