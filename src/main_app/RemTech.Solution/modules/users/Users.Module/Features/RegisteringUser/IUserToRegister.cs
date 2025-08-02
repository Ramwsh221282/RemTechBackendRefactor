using Users.Module.Features.RegisteringUser.Storage;

namespace Users.Module.Features.RegisteringUser;

internal interface IUserToRegister
{
    Task<bool> Register(IUsersStorage storage, CancellationToken ct = default);
}
