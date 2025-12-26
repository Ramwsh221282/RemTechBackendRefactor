using Identity.Domain.Users.Aggregate;

namespace Identity.Integrational.Tests.Common.Cases;

public sealed record UserCaseResult(string Login, string Email, string Password, Guid Id)
{
    public UserCaseResult(User user, string password)
        : this(user.Profile.Login.Name, user.Profile.Email.Email, password, user.Id.Id) { }
}
