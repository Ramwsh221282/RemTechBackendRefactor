using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

public sealed class ParsersSource(IParsers parsers, ITransactionalParsers transactional)
    : IParsers,
        ITransactionalParsers
{
    public Task<Status<IParser>> Find(Name name, CancellationToken ct = default) =>
        parsers.Find(name, ct);

    public Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default) =>
        parsers.Find(id, ct);

    public Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    ) => parsers.Find(type, domain, ct);

    Task<Status> IParsers.Add(IParser parser, CancellationToken ct) => parsers.Add(parser, ct);

    Task<ITransactionalParser> ITransactionalParsers.Add(IParser parser, CancellationToken ct) =>
        transactional.Add(parser, ct);

    public void Dispose()
    {
        parsers.Dispose();
        transactional.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await parsers.DisposeAsync();
        await transactional.DisposeAsync();
    }
}
