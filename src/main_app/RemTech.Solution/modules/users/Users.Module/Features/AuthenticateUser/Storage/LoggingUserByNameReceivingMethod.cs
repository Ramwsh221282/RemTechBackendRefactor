using Npgsql;
using Serilog;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class LoggingUserByNameReceivingMethod(
    ILogger logger,
    IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> origin
) : IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand>
{
    public NpgsqlCommand ModifyQuery(NpgsqlCommand query)
    {
        logger.Information("Determined name authorization method.");
        return origin.ModifyQuery(query);
    }
}
