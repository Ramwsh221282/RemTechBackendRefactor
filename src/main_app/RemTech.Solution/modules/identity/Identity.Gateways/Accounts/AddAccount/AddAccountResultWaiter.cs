using Identity.Core;
using Identity.Core.Accounts.Events;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Accounts.AddAccount;

public sealed record AddAccountResultWaiter : IEventHandler<AccountRegisteredEvent>
{
    private Result<AddAccountResponse> _result = 
        Result.Failure<AddAccountResponse>(Error.Application("Операция не выполнена."));
    
    public void ReactOnEvent(AccountRegisteredEvent @event) =>
        _result = Result.Success(new AddAccountResponse(@event.Id));

    public Result<AddAccountResponse> ReadResult() =>
        _result;
}