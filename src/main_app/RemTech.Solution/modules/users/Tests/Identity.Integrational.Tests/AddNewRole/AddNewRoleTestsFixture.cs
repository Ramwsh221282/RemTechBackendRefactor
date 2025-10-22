using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.UseCases.AddNewRole;
using Identity.Domain.Roles.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.Integrational.Tests.AddNewRole;

public sealed class AddNewRoleTestsFixture
{
    private readonly IServiceProvider _sp;

    public AddNewRoleTestsFixture(IdentityTestApplicationFactory factory)
    {
        _sp = factory.Services;
    }

    public async Task<Status<IdentityRole>> CreateRole(string name)
    {
        AddRoleCommand command = new(name);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<AddRoleCommand, Status<IdentityRole>>>()
            .Handle(command);
    }

    public async Task<IdentityRole?> FindRole(string name)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope.GetService<IRolesStorage>().Get(RoleName.Create(name));
    }

    public async Task<IdentityRole?> FindRole(Guid id)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope.GetService<IRolesStorage>().Get(RoleId.Create(id));
    }
}
