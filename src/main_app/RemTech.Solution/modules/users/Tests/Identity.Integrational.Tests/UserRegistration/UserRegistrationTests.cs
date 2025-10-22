using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserRegistration;

public sealed class UserRegistrationTests(IdentityTestApplicationFactory factory)
    : IClassFixture<IdentityTestApplicationFactory>,
        IAsyncLifetime
{
    private readonly UserRegistrationTestsFixture _fixture = new(factory);

    [Fact]
    private async Task Create_User_Success()
    {
        string login = "myLogin";
        string email = "myEmail@mail.com";
        string password = "myPassword!23";
        Status<IdentityUser> registration = await _fixture.RegisterUser(login, email, password);
        Assert.True(registration.IsSuccess);
    }

    [Fact]
    private async Task Create_User_Failure_Login_Empty()
    {
        string login = "   ";
        string email = "myEmail@mail.com";
        string password = "myPassword!23";
        Status<IdentityUser> registration = await _fixture.RegisterUser(login, email, password);
        Assert.True(registration.IsFailure);
    }

    [Fact]
    private async Task Create_User_Failure_Email_Empty()
    {
        string login = "myLogin";
        string email = "  ";
        string password = "myPassword!23";
        Status<IdentityUser> registration = await _fixture.RegisterUser(login, email, password);
        Assert.True(registration.IsFailure);
    }

    [Theory]
    [InlineData("someEmail")]
    [InlineData("@emailSome")]
    [InlineData("mail@some")]
    private async Task Create_User_Failure_Email_Invalid_Format(string email)
    {
        string login = "myLogin";
        string password = "myPassword!23";
        Status<IdentityUser> registration = await _fixture.RegisterUser(login, email, password);
        Assert.True(registration.IsFailure);
    }

    public async Task InitializeAsync()
    {
        await _fixture.CreateDefaultRoles();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
