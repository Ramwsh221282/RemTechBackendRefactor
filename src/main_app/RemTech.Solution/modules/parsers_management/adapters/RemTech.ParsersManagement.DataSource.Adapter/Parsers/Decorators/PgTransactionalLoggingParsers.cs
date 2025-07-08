using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgTransactionalLoggingParsers(
    ICustomLogger logger,
    ITransactionalParsers origin
) : ITransactionalParsers
{
    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }

    public async Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default)
    {
        ITransactionalParser transactional = await origin.Add(parser, ct);
        return new TransactionalLoggingParser(logger, transactional);
    }
}
