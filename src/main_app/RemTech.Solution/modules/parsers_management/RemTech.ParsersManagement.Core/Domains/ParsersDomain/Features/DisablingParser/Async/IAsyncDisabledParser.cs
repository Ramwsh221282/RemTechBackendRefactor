using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async;

public interface IAsyncDisabledParser
{
    Task<Status<IParser>> Disable(AsyncDisableParser disable, CancellationToken ct = default);
}
