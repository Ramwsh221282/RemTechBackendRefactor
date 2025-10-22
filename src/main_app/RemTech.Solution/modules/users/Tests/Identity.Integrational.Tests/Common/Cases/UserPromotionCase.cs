using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.Common.Cases;

public sealed class UserPromotionCase(
    Guid promoterId,
    string promoterPassword,
    Guid userId,
    string userPassword,
    string roleName
) : UserCase
{
    public async Task<UserCaseResult> Invoke(BaseIdentityModuleTestClass @class)
    {
        Status status = await @class.UseCases.PromoteUserUseCase(
            promoterId,
            promoterPassword,
            userId,
            roleName
        );
        if (status.IsFailure)
            return Failed();

        IdentityUser? created = await @class.UseCases.GetUserUseCase(userId);
        return created == null ? Failed() : Success(created, userPassword);
    }
}
