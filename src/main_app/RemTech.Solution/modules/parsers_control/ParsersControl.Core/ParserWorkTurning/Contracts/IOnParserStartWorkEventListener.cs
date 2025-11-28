using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Contracts;

public interface IOnParserStartWorkEventListener
{
    Task<Result<Unit>> React(ParserWorkTurnerState workTurnerState, CancellationToken ct = default);
}