using Identity.Core.PermissionsModule;
using Tests.ModuleFixtures;

namespace Tests.Identity;

public sealed class RegisterPermissionTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;

    [Fact]
    private async Task Register_Permission_Success()
    {
        string permission = "user.watches.catalogue";
        Result<Permission> result = await _module.RegisterPermission(permission);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Register_Permission_Duplicate_Name_Failure()
    {
        string permission = "user.watches.catalogue";
        Result<Permission> success = await _module.RegisterPermission(permission);
        Result<Permission> failure = await _module.RegisterPermission(permission);
        Assert.True(success.IsSuccess);
        Assert.True(failure.IsFailure);
    }
}