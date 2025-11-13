using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class RegisterPermissionTests(IdentityPersistenceTestsFixture fixture) : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Register_Permission_Success()
    {
        string permission = "user.watches.catalogue";
        Result<Permission> result = await RegisterPermission(permission);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Register_Permission_Duplicate_Name_Failure()
    {
        string permission = "user.watches.catalogue";
        Result<Permission> success = await RegisterPermission(permission);
        Result<Permission> failure = await RegisterPermission(permission);
        Assert.True(success.IsSuccess);
        Assert.True(failure.IsFailure);
    }

    private async Task<Result<Permission>> RegisterPermission(string name)
    {
        await using AsyncServiceScope scope = fixture.Scope();
        CancellationToken ct = CancellationToken.None;
        RegisterPermissionArgs args = new(name, ct);
        RegisterPermission useCase = scope.Resolve<RegisterPermission>();
        return await useCase(args);
    }
}