using Identity.Domain.Roles;
using Identity.Domain.Roles.UseCases.AddNewRole;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.UserRegistration;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.Integrational.Tests.UserRegistration;

public sealed class UserRegistrationTestsFixture(IdentityTestApplicationFactory factory)
{
    private readonly IServiceProvider _sp = factory.Services;

    public async Task<Status> CreateDefaultRoles()
    {
        string[] roles = [RoleName.Admin.Value, RoleName.User.Value, RoleName.Root.Value];
        foreach (string role in roles)
        {
            AddRoleCommand command = new(role);
            await using AsyncServiceScope scope = _sp.CreateAsyncScope();

            await scope
                .GetService<ICommandHandler<AddRoleCommand, Status<IdentityRole>>>()
                .Handle(command);
        }

        return Status.Success();
    }

    public async Task<Status<IdentityUser>> RegisterUser(
        string login,
        string email,
        string password
    )
    {
        UserRegistrationCommand command = new(login, email, password);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        ICommandHandler<UserRegistrationCommand, Status<IdentityUser>> handler = scope.GetService<
            ICommandHandler<UserRegistrationCommand, Status<IdentityUser>>
        >();
        return await handler.Handle(command);
    }
}
