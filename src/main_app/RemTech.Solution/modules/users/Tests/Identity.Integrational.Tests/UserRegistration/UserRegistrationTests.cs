using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserRegistration;

public sealed class UserRegistrationTests(IdentityTestApplicationFactory factory)
    : IClassFixture<IdentityTestApplicationFactory>
{
    private readonly UserRegistrationTestsFixture _fixture = new(factory);

    [Fact]
    public async Task Create_User_Failure_Role_Not_Exists()
    {
        string login = "myLogin";
        string email = "myEmail@mail.com";
        string password = "myPassword!23";
        Status<IdentityUser> registration = await _fixture.RegisterUser(login, email, password);
        Assert.True(registration.IsFailure);
    }
}
