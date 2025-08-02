using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class PasswordHashingUsersStorage(StringHash hash, IUsersStorage origin)
    : IUsersStorage
{
    public Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        string hashedPassword = hash.Hash(password);
        return origin.Save(name, email, hashedPassword, ct);
    }
}
