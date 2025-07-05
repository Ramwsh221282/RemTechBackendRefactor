using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

public interface ITransactionalParsers
{
    Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default);
}
