using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning.Contracts;

public interface IOnParserDisabledEventListener
{
    Task<Result<Unit>> React(ParserWorkTurnerState workTurnerState, CancellationToken ct = default);
}