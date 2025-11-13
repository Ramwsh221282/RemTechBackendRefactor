using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.PermissionsModule.Events;

public sealed record PermissionRenamed(PermissionSnapshot Snapshot) : Notification;