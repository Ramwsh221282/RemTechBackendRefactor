using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Roles.UseCases.AddNewRole;

public sealed record AddRoleCommand(string RoleName) : ICommand;
