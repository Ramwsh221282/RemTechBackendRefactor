using Identity.Domain.Users.Aggregate;
using Identity.Integrational.Tests.Common;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.UserRemovesUser;

public sealed class UserRemoveUserFixture(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    [Fact]
    private async Task Default_User_Removes_Default_User_Failure()
    {
        IdentityUser user1 = await UseCases.RegisterUserUseCase(
            "User #1",
            "userFirst@mail.com",
            "UserFirst!23"
        );

        IdentityUser user2 = await UseCases.RegisterUserUseCase(
            "User #2",
            "userSecond@mail.com",
            "UserSecond!23"
        );

        Guid removerId = user1.Id.Id;
        Guid removalId = user2.Id.Id;

        Status<IdentityUser> removed = await UseCases.RemoveUserUseCase(removerId, removalId);
        Assert.True(removed.IsFailure);

        IdentityUser? removedRead = await UseCases.GetUserUseCase(removalId);
        Assert.NotNull(removedRead);
    }
}
