using Identity.Domain.Users.Aggregate;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class ManageUserProfileUniqueAttributes(
    IUserEmailUnique emailUnique,
    IUserLoginUnique loginUnique
) : IManageUserProfileUniqueAttributes
{
    public async Task<Status> HasUniqueProfileAttributes(User user, CancellationToken ct = default)
    {
        var profile = user.Profile;
        var uniqueLogin = await emailUnique.Unique(profile.Email, ct);
        var uniqueEmail = await loginUnique.Unique(profile.Login, ct);

        return (uniqueLogin.IsFailure, uniqueEmail.IsFailure) switch
        {
            (true, true) => Status.Combined([uniqueLogin, uniqueEmail]),
            (true, false) => uniqueLogin,
            (false, true) => uniqueEmail,
            _ => Status.Success(),
        };
    }
}
