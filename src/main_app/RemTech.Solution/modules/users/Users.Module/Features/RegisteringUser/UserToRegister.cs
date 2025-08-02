using Users.Module.Features.RegisteringUser.Storage;

namespace Users.Module.Features.RegisteringUser;

internal sealed class UserToRegister(string name, string email, string password) : IUserToRegister
{
    public Task<bool> Register(IUsersStorage storage, CancellationToken ct = default) =>
        storage.Save(name, email, password, ct);
}
