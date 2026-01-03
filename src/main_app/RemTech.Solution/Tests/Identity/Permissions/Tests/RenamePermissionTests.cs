using Identity.Application.Permissions;
using Identity.Domain;
using Identity.Domain.Permissions;
using Identity.Gateways.Permissions.AddPermission;
using Identity.Gateways.Permissions.RenamePermission;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Permissions.Fixtures;

namespace Tests.Identity.Permissions.Tests;

public sealed class RenamePermissionTests(PermissionsFixture fixture) : IClassFixture<PermissionsFixture>
{
    private readonly PermissionsFeaturesFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Rename_Permission_Ensure_Renamed()
    {
        const string defaultName = "can.read.catalogue";
        const string newName = "can.edit.catalogue";
        Result<AddPermissionResponse> permission = await _facade.AddPermission(defaultName);
        Assert.True(permission.IsSuccess);
        Guid id = permission.Value.Id;
        Result<RenamePermissionResponse> renaming = await _facade.RenamePermission(id, newName);
        Assert.True(renaming.IsSuccess);
        Permission? fromDb = await _facade.Get(id);
        Assert.NotNull(fromDb);
        Assert.True(fromDb.HasId(id));
        Assert.True(fromDb.HasName(newName));
    }

    [Fact]
    private async Task Rename_Permission_Invalid_Name_Failure()
    {
        const string defaultName = "can.read.catalogue";
        const string newName = "   ";
        Result<AddPermissionResponse> permission = await _facade.AddPermission(defaultName);
        Assert.True(permission.IsSuccess);
        Guid id = permission.Value.Id;
        Result<RenamePermissionResponse> renaming = await _facade.RenamePermission(id, newName);
        Assert.True(renaming.IsFailure);
    }

    [Fact]
    private async Task Rename_Permission_Name_Duplicate_Failure()
    {
        const string defaultName = "can.read.catalogue";
        const string newName = "can.edit.catalogue";
        Result<AddPermissionResponse> permission = await _facade.AddPermission(defaultName);
        Result<AddPermissionResponse> permission2 = await _facade.AddPermission(newName);
        Assert.True(permission.IsSuccess);
        Assert.True(permission2.IsSuccess);
        Guid id = permission.Value.Id;
        Result<RenamePermissionResponse> renaming = await _facade.RenamePermission(id, newName);
        Assert.True(renaming.IsFailure);
    }

    [Fact]
    private async Task Rename_Permission_Not_Found_Error()
    {
        const string newName = "can.edit.catalogue";
        Result<RenamePermissionResponse> renaming = await _facade.RenamePermission(Guid.NewGuid(), newName);
        Assert.True(renaming.IsFailure);
    }
}