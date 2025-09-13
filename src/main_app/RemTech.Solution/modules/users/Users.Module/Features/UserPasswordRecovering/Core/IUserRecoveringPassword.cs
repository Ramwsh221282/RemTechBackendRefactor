using Npgsql;

namespace Users.Module.Features.UserPasswordRecovering.Core;

internal interface IUserRecoveringPassword
{
    void AddTo(NpgsqlCommand command);
}
