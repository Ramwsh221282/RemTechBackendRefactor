using System.Text.Json;
using Identity.Application.AccountTickets;
using Identity.Application.AccountTickets.Decorators;
using Identity.Contracts.Accounts;
using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using Identity.Gateways.AccountTickets.Decorators;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.AccountTickets.OnAccountTicketPasswordResetRequired;

public sealed class AddAccountTicketOnAccountPasswordResetRequired
(Serilog.ILogger logger,
    IOnAccountTicketCreatedEventListener listener,
    IAccountTicketsStorage storage)
    : IOnAccountPasswordResetRequiredListener
{
    public const string Type = "account.password.reset.required";
    
    public async Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        Guid id = Guid.NewGuid();
        string payload = CreatePayload(id, data);
        AccountTicketData ticketData = new(
            Id: Guid.NewGuid(),
            AccountId: data.Id,
            Type: Type,
            Payload: payload,
            Created: DateTime.UtcNow,
            Finished: null
            );

        IAccountTicket ticket = new LoggingAccountTicket(logger, new ValidTicket(
            new PersistingTicket(
                new AccountTicket(ticketData)
                    .AddListener(listener))));

        Result<Unit> result = await ticket.Register(storage, ct);
        return result.Value;
    }

    private static string CreatePayload(Guid id, AccountData data)
    {
        object payload = new
        {
            account_email = data.Email,
            ticket_id = id
        };
        return JsonSerializer.Serialize(payload);
    }
}