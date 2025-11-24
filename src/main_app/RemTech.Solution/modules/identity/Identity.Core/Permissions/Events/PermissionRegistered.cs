namespace Identity.Core.Permissions.Events;

public sealed record PermissionRegistered(Guid Id, string Name) : Event;