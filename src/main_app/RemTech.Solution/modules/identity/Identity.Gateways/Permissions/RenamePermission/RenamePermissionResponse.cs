using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed class RenamePermissionResponse : IResponse
{
    public Guid Id { get; private set; } = Guid.Empty;
    public void Add(Guid id) =>  Id = id;
}