using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Contracts;

public interface IOnPermanentStopWorkEventListener
{
    Task<Result<Unit>> React(ParserWorkTurnerState state, CancellationToken ct = default);
}