using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using Identity.Integrational.Tests.Common;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.AddNewRole;

public sealed class AddNewRoleTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    [Fact]
    private async Task Add_New_Role_Success()
    {
        string name = "My Custom Role";
        Status<IdentityRole> result = await UseCases.CreateRoleUseCase(name);
        Assert.True(result.IsSuccess);

        IdentityRole? created = await UseCases.FindRoleUseCase(name);

        Assert.NotNull(created);
        Assert.Equal(result.Value.Name.Value, created.Name.Value);
        Assert.Equal(result.Value.Id.Value, created.Id.Value);
    }

    [Fact]
    private async Task Add_New_Role_Empty_Name_Failure()
    {
        string name = "  ";
        Status<IdentityRole> result = await UseCases.CreateRoleUseCase(name);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Add_New_Role_Long_Name_Failure()
    {
        string name = string.Join(" ,", Enumerable.Range(0, 200).Select(i => i.ToString()));
        Status<IdentityRole> result = await UseCases.CreateRoleUseCase(name);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Duplicate_Role_Failure()
    {
        string name = IdentityRole.Create(RoleName.User).Name.Value;
        await UseCases.CreateRoleUseCase(name);
        Status<IdentityRole> result = await UseCases.CreateRoleUseCase(name);
        Assert.False(result.IsSuccess);
    }
}
