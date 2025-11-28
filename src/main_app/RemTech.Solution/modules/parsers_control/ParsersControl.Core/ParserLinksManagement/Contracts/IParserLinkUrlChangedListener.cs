using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public interface IParserLinkUrlChangedListener
{
    Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default);
}