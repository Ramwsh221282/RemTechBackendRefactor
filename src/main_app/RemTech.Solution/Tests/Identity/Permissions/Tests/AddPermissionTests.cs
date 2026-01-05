using Identity.Application.Permissions;
using Identity.Domain;
using Identity.Domain.Permissions;
using Identity.Gateways.Permissions.AddPermission;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Permissions.Fixtures;

namespace Tests.Identity.Permissions.Tests;

public sealed class AddPermissionTests(PermissionsFixture fixture) 
    : IClassFixture<PermissionsFixture>
{
    private readonly PermissionsFeaturesFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Add_Permission_Success()
    {
        const string name = "can.read.catalogue";
        Result<AddPermissionResponse> result = await _facade.AddPermission(name);
        Assert.True(result.IsSuccess);
        Guid id = result.Value.Id;
        Permission? permission = await _facade.Get(id);
        Assert.NotNull(permission);
    }

    [Fact]
    private async Task Add_Permission_Name_Duplicate()
    {
        const string name = "can.read.catalogue"; 
        await _facade.AddPermission(name);
        Result<AddPermissionResponse> result = await _facade.AddPermission(name);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Permission_Invalid_Name_Failure()
    {
        const string name = "   "; 
        await _facade.AddPermission(name);
        Result<AddPermissionResponse> result = await _facade.AddPermission(name);
        Assert.True(result.IsFailure);
    }
}