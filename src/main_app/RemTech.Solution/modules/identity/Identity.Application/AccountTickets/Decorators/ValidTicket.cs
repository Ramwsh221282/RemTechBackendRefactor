using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountTickets.Decorators;

public sealed class ValidTicket(IAccountTicket ticket) : AccountTicketEnvelope(ticket)
{
    private readonly IAccountTicket _ticket = ticket;

    public override Task<Result<Unit>> Register(IAccountTicketsStorage storage, CancellationToken ct)
    {
        Result<Unit> validation = Validate();
        return validation.IsFailure 
            ? Task.FromResult<Result<Unit>>(validation.Error) 
            : _ticket.Register(storage, ct);
    }

    private Result<Unit> Validate()
    {
        const string idName = "Идентификатор заявки";
        const string accountIdName = "Идентификатор учетной записи";
        const string typeName = "тип учетной записи";
        const int maxTypeLength = 256;
        List<string> errors = [];
        AccountTicketData data = _ticket.Representation();
        if (data.Id == Guid.Empty) errors.Add(Error.NotSet(idName));
        if (data.AccountId == Guid.Empty) errors.Add(Error.NotSet(accountIdName));
        if (string.IsNullOrWhiteSpace(data.Type)) errors.Add(Error.NotSet(typeName));
        if (data.Type.Length > maxTypeLength) errors.Add(Error.GreaterThan(typeName, maxTypeLength));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }
}