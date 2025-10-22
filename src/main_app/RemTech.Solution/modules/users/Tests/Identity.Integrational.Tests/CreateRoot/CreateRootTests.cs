using Identity.Domain.Users.Aggregate;
using Identity.Integrational.Tests.Common;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.CreateRoot;

public sealed class CreateRootTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    [Fact]
    private async Task Create_Root_User_Success()
    {
        string login = "rootUserLogin";
        string password = "rootUserPassword!23";
        string email = "rootUser@email.com";

        Status<IdentityUser> root = await UseCases.CreateRootUserUseCase(login, email, password);

        Assert.True(root.IsSuccess);

        IdentityUser? created = await UseCases.GetUserUseCase(root.Value.Id.Id);

        Assert.NotNull(created);
        Assert.Equal(login, created.Profile.Login.Name);
        Assert.Equal(email, created.Profile.Email.Email);
        Assert.Equal(root.Value.Id, created.Id);
        Assert.NotEqual(password, created.Profile.Password.Password);
    }
}
