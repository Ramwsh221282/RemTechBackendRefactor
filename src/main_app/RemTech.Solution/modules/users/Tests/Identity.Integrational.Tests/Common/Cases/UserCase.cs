using Identity.Domain.Users.Aggregate;

namespace Identity.Integrational.Tests.Common.Cases;

public class UserCase
{
    protected UserCaseResult Failed() => new(string.Empty, string.Empty, string.Empty, Guid.Empty);

    protected UserCaseResult Success(IdentityUser user, string password) =>
        new(user.Profile.Login.Name, user.Profile.Email.Email, password, user.Id.Id);
}
