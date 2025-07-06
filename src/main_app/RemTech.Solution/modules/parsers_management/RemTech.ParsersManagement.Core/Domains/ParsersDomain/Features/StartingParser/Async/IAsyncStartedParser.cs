using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async;

public interface IAsyncStartedParser
{
    Task<Status<IParser>> StartedAsync(AsyncStartParser start, CancellationToken ct = default);
}
