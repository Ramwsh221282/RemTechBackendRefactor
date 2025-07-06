using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

public sealed class MokTransactionalParsers(MokValidParsers dataSource) : ITransactionalParsers
{
    public Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default)
    {
        ITransactionalParser transactional = new MokTransactionalParser(parser, dataSource);
        return Task.FromResult(transactional);
    }

    public void Dispose() { }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
