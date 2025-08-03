using Npgsql;
using Users.Module.Features.AuthenticateUser.Exceptions;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class UnknownUserReceivingMethod
    : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    public NpgsqlCommand ModifyQuery(NpgsqlCommand query) =>
        throw new UnableToDetermineUserGetMethodException();
}
