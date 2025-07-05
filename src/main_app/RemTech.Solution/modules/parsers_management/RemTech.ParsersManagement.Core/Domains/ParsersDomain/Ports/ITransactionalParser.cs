using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

public interface ITransactionalParser : IParser, IDisposable, IAsyncDisposable
{
    Task<Status> Save(CancellationToken ct = default);
}
