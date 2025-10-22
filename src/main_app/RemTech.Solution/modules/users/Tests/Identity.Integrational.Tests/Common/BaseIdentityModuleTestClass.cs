using Identity.Domain.Roles.ValueObjects;
using Identity.Integrational.Tests.Common.Cases;

namespace Identity.Integrational.Tests.Common;

public class BaseIdentityModuleTestClass(IdentityTestApplicationFactory factory)
    : IClassFixture<IdentityTestApplicationFactory>,
        IAsyncLifetime
{
    public readonly IdentityModuleUseCases UseCases = new(factory);

    public virtual async Task InitializeAsync() => await UseCases.SeedRoles();

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task<UserCaseResult> InitUser(
        string userLogin,
        string userEmail,
        string userPassword
    ) => await new UserCreationCase(userLogin, userEmail, userPassword).Invoke(this);

    protected async Task<UserCaseResult> InitRoot(
        string userLogin,
        string userEmail,
        string userPassword
    ) => await new RootCreationCase(userLogin, userEmail, userPassword).Invoke(this);

    protected async Task<UserCaseResult> InitAdmin(UserCaseResult root, UserCaseResult user) =>
        await new UserPromotionCase(
            root.Id,
            root.Password,
            user.Id,
            user.Password,
            RoleName.Admin.Value
        ).Invoke(this);
}
