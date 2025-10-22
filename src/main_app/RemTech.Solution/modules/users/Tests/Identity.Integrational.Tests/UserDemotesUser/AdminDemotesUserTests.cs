using Identity.Domain.Roles.ValueObjects;
using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserDemotesUser;

public class AdminDemotesUserTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private UserCaseResult _admin = null!;
    private UserCaseResult _user = null!;
    private UserCaseResult _secondAdmin = null!;
    private UserCaseResult _root = null!;
    private readonly RoleName[] _roleNames = [RoleName.User, RoleName.Admin, RoleName.Root];

    [Fact]
    private async Task Admin_Removes_Permission_From_User_Failure()
    {
        foreach (RoleName roleName in _roleNames)
        {
            Status demotion = await UseCases.DemoteUserUseCase(
                _admin.Id,
                _admin.Password,
                _user.Id,
                roleName.Value
            );

            Assert.True(demotion.IsFailure);
        }
    }

    [Fact]
    private async Task Admin_Removes_Permission_From_Admin_Failure()
    {
        foreach (RoleName roleName in _roleNames)
        {
            Status demotion = await UseCases.DemoteUserUseCase(
                _admin.Id,
                _admin.Password,
                _secondAdmin.Id,
                roleName.Value
            );

            Assert.True(demotion.IsFailure);
        }
    }

    [Fact]
    private async Task Admin_Removes_Permission_From_Root_Failure()
    {
        foreach (RoleName roleName in _roleNames)
        {
            Status demotion = await UseCases.DemoteUserUseCase(
                _admin.Id,
                _admin.Password,
                _root.Id,
                roleName.Value
            );

            Assert.True(demotion.IsFailure);
        }
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _root = await InitRoot("rootLogin", "rootEmail@mail.com", "rootPassword!23");
        _user = await InitUser("userLogin", "userEmail@mail.com", "userPassword!23");
        var admin = await InitUser("admin1Login", "admin1mail@com", "admin1Password!23");
        var secondAdmin = await InitUser("admin2Login", "admin2mail@com", "admin2Password!23");
        _admin = await InitAdmin(_root, admin);
        _secondAdmin = await InitAdmin(_root, secondAdmin);
    }
}
