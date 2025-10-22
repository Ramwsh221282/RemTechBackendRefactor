using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Integrational.Tests.Common;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserPromotesUser;

public sealed class UserPromotesUserCommandTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    [Fact]
    private async Task Root_Promotes_User_To_Admin_Success()
    {
        string rootLogin = "rootLogin";
        string rootPassword = "rootPassword!23";
        string rootEmail = "root@email.com";
        await UseCases.CreateRootUserUseCase(rootLogin, rootEmail, rootPassword);

        IdentityUser? root = await UseCases.GetUserByEmailUserCase(rootEmail);
        Assert.NotNull(root);

        string defaultUserLogin = "defaultUser";
        string defaultPassword = "defaultPassword!23";
        string defaultEmail = "defaultEmail@mail.com";
        await UseCases.RegisterUserUseCase(defaultUserLogin, defaultEmail, defaultPassword);

        IdentityUser? defaultUser = await UseCases.GetUserByEmailUserCase(defaultEmail);
        Assert.NotNull(defaultUser);

        Status promotion = await UseCases.PromoteUserUseCase(
            root.Id.Id,
            rootPassword,
            defaultUser.Id.Id,
            RoleName.Admin.Value
        );

        Assert.True(promotion.IsSuccess);

        IdentityUser? promotedAdmin = await UseCases.GetUserUseCase(defaultUser.Id.Id);

        Assert.NotNull(promotedAdmin);
        Assert.Equal(defaultUserLogin, promotedAdmin.Profile.Login.Name);
        Assert.Equal(defaultEmail, promotedAdmin.Profile.Email.Email);
        Assert.True(promotedAdmin.Roles.Roles.Any(r => r.Name.Value == RoleName.Admin.Value));
        Assert.True(promotedAdmin.Roles.Roles.Any(r => r.Name.Value == RoleName.User.Value));
    }

    [Fact]
    private async Task Promotes_User_Same_Role_Twice_Failure()
    {
        string rootLogin = "rootLogin";
        string rootPassword = "rootPassword!23";
        string rootEmail = "root@email.com";
        await UseCases.CreateRootUserUseCase(rootLogin, rootEmail, rootPassword);

        IdentityUser? root = await UseCases.GetUserByEmailUserCase(rootEmail);
        Assert.NotNull(root);
        string defaultUserLogin = "defaultUser";
        string defaultPassword = "defaultPassword!23";
        string defaultEmail = "defaultEmail@mail.com";
        await UseCases.RegisterUserUseCase(defaultUserLogin, defaultEmail, defaultPassword);

        IdentityUser? defaultUser = await UseCases.GetUserByEmailUserCase(defaultEmail);
        Assert.NotNull(defaultUser);

        Status promotion = await UseCases.PromoteUserUseCase(
            root.Id.Id,
            rootPassword,
            defaultUser.Id.Id,
            RoleName.Admin.Value
        );

        Assert.True(promotion.IsSuccess);

        promotion = await UseCases.PromoteUserUseCase(
            root.Id.Id,
            rootPassword,
            defaultUser.Id.Id,
            RoleName.Admin.Value
        );
        Assert.False(promotion.IsSuccess);

        defaultUser = await UseCases.GetUserByEmailUserCase(defaultEmail);
        Assert.NotNull(defaultUser);
        Assert.Equal(2, defaultUser.Roles.Roles.Count());
    }

    [Fact]
    private async Task Admin_Promotes_To_Admin_Failure()
    {
        string rootLogin = "rootLogin";
        string rootPassword = "rootPassword!23";
        string rootEmail = "root@email.com";
        await UseCases.CreateRootUserUseCase(rootLogin, rootEmail, rootPassword);
        IdentityUser? root = await UseCases.GetUserByEmailUserCase(rootEmail);
        Assert.NotNull(root);

        string defaultUserLogin = "defaultUser";
        string defaultPassword = "defaultPassword!23";
        string defaultEmail = "defaultEmail@mail.com";
        await UseCases.RegisterUserUseCase(defaultUserLogin, defaultEmail, defaultPassword);
        IdentityUser? defaultUser = await UseCases.GetUserByEmailUserCase(defaultEmail);
        Assert.NotNull(defaultUser);

        string justUserLogin = "justUserLogin";
        string justPassword = "justUserPassword!23";
        string justUserEmail = "justUserEmail@email.com";
        await UseCases.RegisterUserUseCase(justUserLogin, justUserEmail, justPassword);
        IdentityUser? justUser = await UseCases.GetUserByEmailUserCase(justUserEmail);
        Assert.NotNull(justUser);

        await UseCases.PromoteUserUseCase(
            root.Id.Id,
            rootPassword,
            defaultUser.Id.Id,
            RoleName.Admin.Value
        );

        Status promotion = await UseCases.PromoteUserUseCase(
            defaultUser.Id.Id,
            defaultPassword,
            justUser.Id.Id,
            RoleName.Admin.Value
        );

        Assert.True(promotion.IsFailure);
    }
}
