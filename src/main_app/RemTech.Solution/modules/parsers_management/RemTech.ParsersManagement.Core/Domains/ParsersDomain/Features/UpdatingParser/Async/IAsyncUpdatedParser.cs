using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async;

public interface IAsyncUpdatedParser
{
    Task<Status<IParser>> Update(AsyncUpdateParser update, CancellationToken ct = default);
}
