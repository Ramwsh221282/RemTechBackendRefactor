using Users.Module.Features.RegisteringUser.Storage;

namespace Users.Module.Features.RegisteringUser;

internal interface IUserToRegister
{
    Task<bool> Register(INewUsersStorage storage, CancellationToken ct = default);
}
