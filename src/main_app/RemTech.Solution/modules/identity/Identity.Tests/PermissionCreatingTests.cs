using Identity.Domain.Permissions;
using Identity.Domain.Permissions.Features.AddPermissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

public sealed class PermissionCreatingTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
    private IServiceProvider Services { get; } = factory.Services;
    
    [Fact]
    private async Task Add_Permissions_List_Success()
    {
        IEnumerable<AddPermissionCommandPayload> payload =
        [
            new("permission.first", "Test first permission"),
            new("permission.second", "Test second permission"),
            new("permission.third", "Test third permission"),
            new("permission.fourth", "Test fourth permission"),
        ];
        
        Result<IEnumerable<Permission>> result = await Services.AddPermissions(payload);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Permissions_List_With_Duplicates_Failure()
    {
        IEnumerable<AddPermissionCommandPayload> payload1 =
        [
            new("permission.first", "Test first permission"),
            new("permission.second", "Test second permission"),
            new("permission.third", "Test third permission"),
            new("permission.fourth", "Test fourth permission"),
        ];
        
        Result<IEnumerable<Permission>> result1 = await Services.AddPermissions(payload1);
        Assert.True(result1.IsSuccess);
        
        IEnumerable<AddPermissionCommandPayload> payload2 =
        [
            new("permission.third", "Test third permission"),
            new("permission.fourth", "Test fourth permission"),
            new("permission.fifth", "Test fifth permission"),
            new("permission.sixth", "Test sixth permission"),
        ];
        
        Result<IEnumerable<Permission>> result2 = await Services.AddPermissions(payload2);
        Assert.True(result2.IsFailure);
    }
}