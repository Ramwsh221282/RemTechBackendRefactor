using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountTickets;

public sealed class AccountTicket(AccountTicketData data) : IAccountTicket
{
    private readonly IOnAccountTicketCreatedEventListener _onCreated = new NoneOnAccountTicketCreatedEventListener();
    
    public async Task<Result<Unit>> Register(IAccountTicketsStorage storage, CancellationToken ct)
    {
        if (data.Finished.HasValue) return Error.Conflict("Нельзя создать выполненную заявку.");
        Result<Unit> reacting = await _onCreated.React(data, ct);
        if (reacting.IsFailure) return reacting.Error;
        return Unit.Value;
    }

    public AccountTicket AddListener(IOnAccountTicketCreatedEventListener listener)
    {
        return new AccountTicket(data, listener);
    }
    
    public AccountTicketData Representation() => data;

    private AccountTicket(
        AccountTicketData data,
        IOnAccountTicketCreatedEventListener? onCreated) 
        : this(data)
    {
        _onCreated = onCreated ?? new NoneOnAccountTicketCreatedEventListener();
        
    }
}