using Identity.Domain.Roles;
using Identity.Domain.Roles.UseCases.AddNewRole;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.Seeding;

public sealed class RolesSeeding(ICommandHandler<AddRoleCommand, Status<IdentityRole>> handler)
    : IRolesSeeding
{
    private readonly List<string> _roleNames = [];

    public void AddRole(string roleName) => _roleNames.Add(roleName);

    public async Task Seed()
    {
        foreach (string role in _roleNames)
        {
            var command = new AddRoleCommand(role);
            await handler.Handle(command);
        }
    }
}
