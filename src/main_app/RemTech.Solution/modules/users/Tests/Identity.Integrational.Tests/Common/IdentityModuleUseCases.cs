using Identity.Domain.Roles;
using Identity.Domain.Roles.Ports;
using Identity.Domain.Roles.UseCases.AddNewRole;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Aggregate.ValueObjects;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.ConfirmEmailTicket;
using Identity.Domain.Users.UseCases.CreateEmailConfirmationTicket;
using Identity.Domain.Users.UseCases.CreateRoot;
using Identity.Domain.Users.UseCases.UserDemotesUser;
using Identity.Domain.Users.UseCases.UserPromotesUser;
using Identity.Domain.Users.UseCases.UserRegistration;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.Integrational.Tests.Common;

public sealed class IdentityModuleUseCases(IdentityTestApplicationFactory factory)
{
    private readonly IServiceProvider _sp = factory.Services;

    public async Task SeedRoles()
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
    }

    public async Task<Status<IdentityRole>> CreateRoleUseCase(string name)
    {
        AddRoleCommand command = new(name);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<AddRoleCommand, Status<IdentityRole>>>()
            .Handle(command);
    }

    public async Task<IdentityRole?> FindRoleUseCase(string name)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope.GetService<IRolesStorage>().Get(RoleName.Create(name));
    }

    public async Task<IdentityRole?> FindRoleUseCase(Guid id)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope.GetService<IRolesStorage>().Get(RoleId.Create(id));
    }

    public async Task<User?> GetUserUseCase(Guid userId)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        IUsersStorage users = scope.ServiceProvider.GetRequiredService<IUsersStorage>();
        return await users.Get(UserId.Create(userId));
    }

    public async Task<User?> GetUserByLoginUserCase(string login)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        IUsersStorage users = scope.ServiceProvider.GetRequiredService<IUsersStorage>();
        return await users.Get(UserLogin.Create(login));
    }

    public async Task<User?> GetUserByEmailUserCase(string email)
    {
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        IUsersStorage users = scope.ServiceProvider.GetRequiredService<IUsersStorage>();
        return await users.Get(UserEmail.Create(email));
    }

    public async Task<Status<User>> RegisterUserUseCase(string login, string email, string password)
    {
        UserRegistrationCommand command = new(login, email, password);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        ICommandHandler<UserRegistrationCommand, Status<User>> handler = scope.GetService<
            ICommandHandler<UserRegistrationCommand, Status<User>>
        >();
        return await handler.Handle(command);
    }

    public async Task<Status<User>> DemoteUserUseCase(
        Guid demoterId,
        string demoterPassword,
        Guid demoteId,
        string roleName
    )
    {
        UserDemotesUserCommand command = new(demoterId, demoteId, demoterPassword, roleName);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<UserDemotesUserCommand, Status<User>>>()
            .Handle(command);
    }

    public async Task<Status<User>> CreateRootUserUseCase(
        string login,
        string email,
        string password
    )
    {
        CreateRootUserCommand command = new(email, login, password);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<CreateRootUserCommand, Status<User>>>()
            .Handle(command);
    }

    public async Task<Status<User>> CreateEmailConfirmationUseCase(Guid id, string password)
    {
        var command = new CreateEmailConfirmationTicketCommand(id, password);
        await using var scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<CreateEmailConfirmationTicketCommand, Status<User>>>()
            .Handle(command);
    }

    public async Task<Status<User>> ConfirmEmailTicketUseCase(Guid ticketId)
    {
        var command = new ConfirmEmailTicketCommand(ticketId);
        await using var scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<ConfirmEmailTicketCommand, Status<User>>>()
            .Handle(command);
    }

    public async Task<Status<User>> PromoteUserUseCase(
        Guid promoterId,
        string promoterPassword,
        Guid userId,
        string roleName
    )
    {
        UserPromotesUserCommand command = new(promoterId, userId, promoterPassword, roleName);
        await using AsyncServiceScope scope = _sp.CreateAsyncScope();
        return await scope
            .GetService<ICommandHandler<UserPromotesUserCommand, Status<User>>>()
            .Handle(command);
    }
}
