using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.EventListeners.OnStateChanged;

public sealed class NpgSqlOnParserWorkStateChangedEventListener(IParserWorkStatesStorage storage) : IParserStateChangedEventListener
{
    public async Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default)
    {
        await storage.Update(new ParserWorkTurner(state), ct);
        return Unit.Value;
    }
}