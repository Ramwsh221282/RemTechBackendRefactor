using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.UserRegistration;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;

namespace Identity.Integrational.Tests.UserRegistration;

public sealed class UserRegistrationTestsFixture
{
    private readonly IServiceProvider _sp;

    public UserRegistrationTestsFixture(IdentityTestApplicationFactory factory) =>
        _sp = factory.Services;

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
