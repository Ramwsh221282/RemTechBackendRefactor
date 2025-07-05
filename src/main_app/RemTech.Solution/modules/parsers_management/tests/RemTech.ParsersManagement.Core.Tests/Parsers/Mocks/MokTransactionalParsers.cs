using RemTech.ParsersManagement.Core.Domains.ParsersDomain;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Mocks;

public sealed class MokTransactionalParsers(MokValidParsers dataSource) : ITransactionalParsers
{
    public Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default)
    {
        ITransactionalParser transactional = new MokTransactionalParser(parser, dataSource);
        return Task.FromResult(transactional);
    }
}
