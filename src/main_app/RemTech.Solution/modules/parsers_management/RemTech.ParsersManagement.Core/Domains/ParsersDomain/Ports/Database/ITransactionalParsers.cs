using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;

public interface ITransactionalParsers : IDisposable, IAsyncDisposable
{
    Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default);
}
