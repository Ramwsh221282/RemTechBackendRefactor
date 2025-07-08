using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

public sealed class MokValidParsers(MokParsers inner) : IParsers
{
    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        Status<IParser> parser = await inner.Find(name, ct);
        return parser.IsFailure ? parser.Error : new ValidParser(parser.Value);
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        Status<IParser> parser = await inner.Find(id, ct);
        return parser.IsFailure ? parser.Error : new ValidParser(parser.Value);
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        Status<IParser> parser = await inner.Find(type, domain, ct);
        return parser.IsFailure ? parser.Error : new ValidParser(parser.Value);
    }

    public Task<Status> Add(IParser parser, CancellationToken ct = default) =>
        inner.Add(parser, ct);

    public void SaveParser(IParser parser) => inner.SaveParser(parser);

    public bool ContainsParser(IParser parser) => inner.ContainsParser(parser);

    public void Dispose() => inner.Dispose();

    public async ValueTask DisposeAsync() => await inner.DisposeAsync();
}
