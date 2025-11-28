using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Contracts;

public interface IOnParserStartWaitingEventListener
{
    Task<Result<Unit>> React(ParserWorkTurnerState workTurnerState, CancellationToken ct = default);
}