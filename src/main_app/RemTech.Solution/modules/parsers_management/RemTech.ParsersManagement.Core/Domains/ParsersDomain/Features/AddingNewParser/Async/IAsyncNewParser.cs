using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async;

public interface IAsyncNewParser
{
    Task<Status<IParser>> Register(AsyncAddNewParser add, CancellationToken ct = default);
}
