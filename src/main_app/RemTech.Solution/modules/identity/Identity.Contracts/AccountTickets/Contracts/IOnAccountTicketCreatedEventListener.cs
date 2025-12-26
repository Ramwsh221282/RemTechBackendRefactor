using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.AccountTickets.Contracts;

public interface IOnAccountTicketCreatedEventListener
{
    Task<Unit> React(AccountTicketData data, CancellationToken ct = default);
}

public sealed class NoneOnAccountTicketCreatedEventListener : IOnAccountTicketCreatedEventListener
{
    public Task<Unit> React(AccountTicketData data, CancellationToken ct = default)
    {
        return Task.FromResult(Unit.Value);
    }
}