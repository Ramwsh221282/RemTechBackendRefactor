using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.PermissionsModule.Events;

public sealed record PermissionRegistered(PermissionSnapshot Snapshot) : Notification;