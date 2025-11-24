namespace Identity.Core.Permissions.Events;

public sealed record PermissionRenamed(
    Guid Id, 
    string OldName, 
    string NewName) : Event;