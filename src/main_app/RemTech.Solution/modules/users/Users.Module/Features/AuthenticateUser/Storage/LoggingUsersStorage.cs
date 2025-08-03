using Npgsql;
using Serilog;
using Users.Module.Features.AuthenticateUser.Exceptions;

namespace Users.Module.Features.AuthenticateUser.Storage;

internal sealed class LoggingUsersStorage(ILogger logger, IExistingUsersStorage storage)
    : IExistingUsersStorage
{
    public async Task<IExistingUser> Get(
        IUsersReceivingMethod<NpgsqlCommand, NpgsqlCommand> method,
        CancellationToken ct = default
    )
    {
        try
        {
            return await storage.Get(method, ct);
        }
        catch (UserDoesNotExistsException ex)
        {
            string message = ex.Message;
            logger.Error("{Ex}", message);
            throw;
        }
    }
}
