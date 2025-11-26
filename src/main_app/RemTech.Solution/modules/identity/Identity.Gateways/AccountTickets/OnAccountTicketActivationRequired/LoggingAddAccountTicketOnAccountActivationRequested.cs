using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;

public sealed class LoggingAddAccountTicketOnAccountActivationRequested(
    Serilog.ILogger logger,
    IOnAccountActivationRequiredListener origin) :
    IOnAccountActivationRequiredListener
{
    private const string context = "Account tickets module.";
    
    public async Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        logger.Information("{Context} Reacting on account activation ticket required.", context);
        Result<Unit> result = await origin.React(data, ct);
        if (result.IsFailure)
        {
            logger.Information("{Context} ERROR: {Error}", context, result.Error.Message);
            return result.Error;
        }
        
        logger.Information("{Context} processed on account activation ticket required.", context);
        return result;
    }
}