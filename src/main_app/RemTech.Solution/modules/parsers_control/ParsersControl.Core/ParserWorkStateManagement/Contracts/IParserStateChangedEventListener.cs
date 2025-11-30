using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkStateManagement.Contracts;

public interface IParserStateChangedEventListener
{
    public Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default);
}