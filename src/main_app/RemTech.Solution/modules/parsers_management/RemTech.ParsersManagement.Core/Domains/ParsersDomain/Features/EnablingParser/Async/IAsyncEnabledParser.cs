using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async;

public interface IAsyncEnabledParser
{
    Task<Status<IParser>> EnableAsync(AsyncEnableParser enable, CancellationToken ct = default);
}
