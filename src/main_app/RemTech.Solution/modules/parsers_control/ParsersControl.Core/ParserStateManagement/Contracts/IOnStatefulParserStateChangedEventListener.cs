using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserWorkTurning;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStateManagement.Contracts;

public interface IOnStatefulParserStateChangedEventListener
{
    Task<Result<Unit>> React(
        RegisteredParser parser, 
        ParserWorkTurner turner, 
        CancellationToken ct = default);
}