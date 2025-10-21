using Identity.Domain.Roles;
using Identity.Domain.Roles.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.AddNewRole;

public sealed class AddNewRoleTests : IClassFixture<IdentityTestApplicationFactory>
{
    private readonly AddNewRoleTestsFixture _fixture;

    public AddNewRoleTests(IdentityTestApplicationFactory factory)
    {
        _fixture = new AddNewRoleTestsFixture(factory);
    }

    [Fact]
    private async Task Add_New_Role_Success()
    {
        string name = Role.Create(RoleName.User).Name.Value;
        Status<Role> result = await _fixture.CreateRole(name);
        Assert.True(result.IsSuccess);

        Role? created = await _fixture.FindRole(name);
        Assert.NotNull(created);
        Assert.Equal(result.Value.Name.Value, created.Name.Value);
        Assert.Equal(result.Value.Id.Value, created.Id.Value);
    }

    [Fact]
    private async Task Add_New_Role_Empty_Name_Failure()
    {
        string name = "  ";
        Status<Role> result = await _fixture.CreateRole(name);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Add_New_Role_Long_Name_Failure()
    {
        string name = string.Join(" ,", Enumerable.Range(0, 200).Select(i => i.ToString()));
        Status<Role> result = await _fixture.CreateRole(name);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Duplicate_Role_Failure()
    {
        string name = Role.Create(RoleName.User).Name.Value;
        await _fixture.CreateRole(name);
        Status<Role> result = await _fixture.CreateRole(name);
        Assert.False(result.IsSuccess);
    }
}
