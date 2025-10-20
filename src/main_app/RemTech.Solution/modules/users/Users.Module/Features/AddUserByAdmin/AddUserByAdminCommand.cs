using RemTech.Core.Shared.Cqrs;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed record AddUserByAdminCommand(Guid AdditorId, string Email, string Name, string Role)
    : ICommand;
