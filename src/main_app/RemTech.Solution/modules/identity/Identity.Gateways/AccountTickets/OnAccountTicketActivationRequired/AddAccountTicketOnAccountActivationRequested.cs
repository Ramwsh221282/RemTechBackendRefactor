using System.Text.Json;
using Identity.Application.AccountTickets;
using Identity.Application.AccountTickets.Decorators;
using Identity.Contracts.Accounts;
using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using Identity.Gateways.AccountTickets.Decorators;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;

public sealed class AddAccountTicketOnAccountActivationRequested(
    Serilog.ILogger logger,
    IOnAccountTicketCreatedEventListener listener,
    IAccountTicketsStorage storage
    )
    : IOnAccountActivationRequiredListener
{
    public const string Type = "account.activation.ticket.required";
    
    public async Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        Guid id = Guid.NewGuid();
        string payload = CreatePayload(data, id);
        AccountTicketData ticketData = new(id, data.Id, Type, payload, DateTime.UtcNow, null);
        
        IAccountTicket ticket = new LoggingAccountTicket(
            logger, new ValidTicket(
                new PersistingTicket(
                    new AccountTicket(
                        ticketData).AddListener(listener))));
        
        Result<Unit> result = await ticket.Register(storage, ct);
        return result;
    }

    private static string CreatePayload(AccountData data, Guid id)
    {
        object payload = new
        {
            account_email = data.Email,
            ticket_id = id
        };
        return JsonSerializer.Serialize(payload);
    }
}