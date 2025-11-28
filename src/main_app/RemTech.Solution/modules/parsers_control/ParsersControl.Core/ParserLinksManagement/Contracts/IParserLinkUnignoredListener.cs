using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public interface IParserLinkUnignoredListener
{
    Task<Result<Unit>> React(ParserLinkData data, CancellationToken ct = default);
}