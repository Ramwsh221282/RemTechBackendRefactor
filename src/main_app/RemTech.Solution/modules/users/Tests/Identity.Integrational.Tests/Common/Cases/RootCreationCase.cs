using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Integrational.Tests.Common.Cases;

public sealed class RootCreationCase(string login, string email, string password) : UserCase
{
    public async Task<UserCaseResult> Invoke(BaseIdentityModuleTestClass @class)
    {
        Status status = await @class.UseCases.CreateRootUserUseCase(login, email, password);
        if (status.IsFailure)
            return Failed();

        User? created = await @class.UseCases.GetUserByEmailUserCase(email);
        return created == null ? Failed() : Success(created, password);
    }
}
