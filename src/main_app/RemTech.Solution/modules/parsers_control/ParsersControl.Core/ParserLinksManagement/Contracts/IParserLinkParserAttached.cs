using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public interface IParserLinkParserAttached
{
    Task<Result<Unit>> React(Guid parserId, ParserLinkData data, CancellationToken ct = default);
}