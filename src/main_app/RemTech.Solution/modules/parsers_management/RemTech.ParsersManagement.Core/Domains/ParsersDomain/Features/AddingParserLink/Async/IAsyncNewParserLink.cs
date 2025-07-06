using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async;

public interface IAsyncNewParserLink
{
    Task<Status<IParserLink>> AsyncNew(AsyncAddParserLink add, CancellationToken ct = default);
}
