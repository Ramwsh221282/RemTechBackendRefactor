using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Input;

public sealed record UserRegistrationByAdminCommand(
    Guid AdminToken,
    string Email,
    string Name,
    Guid RoleId
) : ICommand;
