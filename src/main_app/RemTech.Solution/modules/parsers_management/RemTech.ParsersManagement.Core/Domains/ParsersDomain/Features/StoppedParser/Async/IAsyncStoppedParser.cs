using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async;

public interface IAsyncStoppedParser
{
    Task<Status<IParser>> AsyncStopped(AsyncStopParser stop, CancellationToken ct = default);
}