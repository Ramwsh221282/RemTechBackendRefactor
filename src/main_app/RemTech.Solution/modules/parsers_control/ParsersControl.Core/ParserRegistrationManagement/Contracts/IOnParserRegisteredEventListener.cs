using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserRegistrationManagement.Contracts;

public interface IOnParserRegisteredEventListener
{
    Task<Result<Unit>> React(ParserData data, CancellationToken ct = default);
}