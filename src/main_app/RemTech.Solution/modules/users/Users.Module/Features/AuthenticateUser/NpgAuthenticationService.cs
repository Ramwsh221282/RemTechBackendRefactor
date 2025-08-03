using Npgsql;
using Users.Module.Features.AuthenticateUser.Exceptions;
using Users.Module.Features.AuthenticateUser.Storage;

namespace Users.Module.Features.AuthenticateUser;

internal sealed class NpgAuthenticationService(
    IExistingUsersStorage storage,
    UserAuthenticationPassword password
)
{
    private IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> _receiving =
        new UnknownUserReceivingMethod();

    public NpgAuthenticationService Method(
        IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> receiving
    )
    {
        _receiving = receiving;
        return this;
    }

    public async Task<IExistingUser> Authenticated(CancellationToken ct = default)
    {
        IExistingUser user = await storage.Get(_receiving, ct);
        return user.OwnsPassword(password) ? user : throw new UserIsNotAuthenticatedException();
    }
}
