using Identity.Application.AccountTickets.Decorators;
using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.AccountTickets.Decorators;

public sealed class LoggingAccountTicket(
    Serilog.ILogger logger, 
    IAccountTicket ticket) 
    : AccountTicketEnvelope(ticket)
{
    private readonly IAccountTicket _ticket = ticket;

    public override async Task<Result<Unit>> Register(
        IAccountTicketsStorage storage,
        CancellationToken ct)
    {
        logger.Information("Registering account ticket");
        Result<Unit> registration = await _ticket.Register(storage, ct);
        if (registration.IsFailure)
        {
            logger.Error("Registering account ticket failed. Error: {Error}", registration.Error.Message);
            return registration.Error;
        }
        
        LogAccountTicket(_ticket);
        return registration;
    }

    private void LogAccountTicket(IAccountTicket ticket)
    {
        AccountTicketData data = ticket.Representation();
        object[] payload = [data.Id, data.AccountId, data.Type, data.Payload];
        logger.Information("""
                           Account ticket info: 
                           ID: {Id}
                           Account Id {AccountId}
                           Type: {Type}
                           Payload: {Payload}
                           """, payload);
    }
}