using Serilog;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class LoggingNewUsersStorage(ILogger logger, INewUsersStorage origin)
    : INewUsersStorage
{
    public async Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "Registering new user. Name - {Name} Email - {Email} Password - {Password}",
            name,
            email,
            password
        );
        bool result = await origin.Save(name, email, password, ct);
        logger.Information("New user created.");
        return result;
    }
}
