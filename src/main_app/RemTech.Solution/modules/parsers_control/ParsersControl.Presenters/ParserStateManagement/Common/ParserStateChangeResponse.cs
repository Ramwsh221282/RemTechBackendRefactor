using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkTurning;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Common;

public sealed class ParserStateChangeResponse : IResponse, IOnStatefulParserStateChangedEventListener
{
    public Guid Id { get; private set; } = Guid.Empty;
    public string State { get; private set; } = string.Empty;
    
    private void AddId(Guid id) =>  Id = id;
    private void AddState(string state) => State = state;
    
    public Task<Result<Unit>> React(RegisteredParser parser, ParserWorkTurner turner, CancellationToken ct = default)
    {
        parser.Write(AddId);
        turner.Write(writeState: AddState);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}