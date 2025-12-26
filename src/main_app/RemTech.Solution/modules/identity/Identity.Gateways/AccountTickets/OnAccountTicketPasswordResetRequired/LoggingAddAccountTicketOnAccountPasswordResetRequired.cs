using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.AccountTickets.OnAccountTicketPasswordResetRequired;

public sealed class LoggingAddAccountTicketOnAccountPasswordResetRequired(
    Serilog.ILogger logger,
    IOnAccountPasswordResetRequiredListener origin) 
    : IOnAccountPasswordResetRequiredListener
{
    private const string Context = nameof(AddAccountTicketOnAccountPasswordResetRequired);
    
    public async Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        logger.Information("{Context} reacting on account password reset requiring", Context);
        Result<Unit> result = await origin.React(data, ct);
        if (result.IsFailure)
        {
            logger.Error("{Context} ERROR: {Error}", Context, result.Error.Message);
            return result.Error;
        }
        
        logger.Information("{Context} reacted on account password reset requiring.", Context);
        return result;
    }
}