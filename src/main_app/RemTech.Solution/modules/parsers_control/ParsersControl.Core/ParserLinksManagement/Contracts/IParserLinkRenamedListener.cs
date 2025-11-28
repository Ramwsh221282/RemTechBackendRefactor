using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public interface IParserLinkRenamedListener
{
    Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default);
}