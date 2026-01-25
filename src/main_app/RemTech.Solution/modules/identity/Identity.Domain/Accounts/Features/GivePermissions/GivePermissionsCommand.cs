using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.GivePermissions;

public sealed record GivePermissionsCommand(Guid AccountId, IEnumerable<GivePermissionsPermissionsPayload> Permissions)
    : ICommand;
