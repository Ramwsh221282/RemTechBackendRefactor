using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;
using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserDemotesUser;

public sealed class UserDemotesUserTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private readonly TestUsersFactory _factory = new(new IdentityModuleUseCases(factory));

    [Fact]
    private async Task User_Removes_Permission_From_User_Failure()
    {
        var user = await _factory.CreateUser("userName", "useremail@mail.com", "userPassword!23");
        var other = await _factory.CreateUser(
            "userName2",
            "seconduseremail@mail.com",
            "userPassword!23"
        );
        Assert.True(user.IsSuccess);
        Assert.True(other.IsSuccess);

        Status demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            other.Value.Id,
            RoleName.User.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            other.Value.Id,
            RoleName.Admin.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            other.Value.Id,
            RoleName.Root.Value
        );
        Assert.True(demotion.IsFailure);
    }

    [Fact]
    private async Task User_Removes_Permission_From_Admin_Failure()
    {
        var user = await _factory.CreateUser("userName", "useremail@mail.com", "userPassword!23");
        var root = await _factory.CreateRoot("rootName", "root@mail.com", "rootPassword!23");
        var other = await _factory.CreateUser(
            "userName2",
            "seconduseremail@mail.com",
            "userPassword!23"
        );
        var admin = await _factory.Promote(root, other, RoleName.Admin.Value);

        Assert.True(user.IsSuccess);
        Assert.True(root.IsSuccess);
        Assert.True(other.IsSuccess);
        Assert.True(admin.IsSuccess);

        Status demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            admin.Value.Id,
            RoleName.User.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            admin.Value.Id,
            RoleName.Admin.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            admin.Value.Id,
            RoleName.Root.Value
        );
        Assert.True(demotion.IsFailure);
    }

    [Fact]
    private async Task User_Removes_Permission_From_Root_Failure()
    {
        var user = await _factory.CreateUser("userName", "useremail@mail.com", "userPassword!23");
        var root = await _factory.CreateRoot("rootName", "root@mail.com", "rootPassword!23");

        Assert.True(user.IsSuccess);
        Assert.True(root.IsSuccess);

        Status demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            root.Value.Id,
            RoleName.User.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            root.Value.Id,
            RoleName.Admin.Value
        );
        Assert.True(demotion.IsFailure);

        demotion = await UseCases.DemoteUserUseCase(
            user.Value.Id,
            user.Value.Password,
            root.Value.Id,
            RoleName.Root.Value
        );
        Assert.True(demotion.IsFailure);
    }
}

public interface ITestUsersFactory
{
    Task<Status<UserCaseResult>> CreateUser(string userName, string userEmail, string userPassword);

    Task<Status<UserCaseResult>> Promote(UserCaseResult root, UserCaseResult user, string roleName);

    Task<Status<UserCaseResult>> CreateRoot(string userName, string userEmail, string userPassword);
}

public sealed class TestUsersFactory : ITestUsersFactory
{
    private readonly IdentityModuleUseCases _useCases;

    public TestUsersFactory(IdentityModuleUseCases useCases) => _useCases = useCases;

    public async Task<Status<UserCaseResult>> CreateUser(
        string userName,
        string userEmail,
        string userPassword
    )
    {
        Status status = await _useCases.RegisterUserUseCase(userName, userEmail, userPassword);
        if (status.IsFailure)
            return status.Error;

        IdentityUser? created = await _useCases.GetUserByEmailUserCase(userEmail);
        return created == null ? Error.NotFound("NF") : new UserCaseResult(created, userPassword);
    }

    public async Task<Status<UserCaseResult>> Promote(
        UserCaseResult root,
        UserCaseResult user,
        string roleName
    )
    {
        Status status = await _useCases.PromoteUserUseCase(
            root.Id,
            root.Password,
            user.Id,
            roleName
        );
        if (status.IsFailure)
            return status.Error;

        IdentityUser? created = await _useCases.GetUserByEmailUserCase(user.Email);
        return created == null ? Error.NotFound("NF") : new UserCaseResult(created, user.Password);
    }

    public async Task<Status<UserCaseResult>> CreateRoot(
        string userName,
        string userEmail,
        string userPassword
    )
    {
        Status status = await _useCases.CreateRootUserUseCase(userName, userEmail, userPassword);
        if (status.IsFailure)
            return status.Error;

        IdentityUser? created = await _useCases.GetUserByEmailUserCase(userEmail);
        return created == null ? Error.NotFound("NF") : new UserCaseResult(created, userPassword);
    }
}
