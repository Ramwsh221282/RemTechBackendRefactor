using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgValidParsers(IParsers origin) : IParsers
{
    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }

    public async Task<Status<IParser>> WrappedOrError(Func<Task<Status<IParser>>> func)
    {
        Status<IParser> founded = await func();
        return founded.IsSuccess ? new ValidParser(founded.Value) : founded.Error;
    }

    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        return await WrappedOrError(() => origin.Find(name, ct));
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        return await WrappedOrError(() => origin.Find(id, ct));
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        return await WrappedOrError(() => origin.Find(type, domain, ct));
    }

    public Task<Status> Add(IParser parser, CancellationToken ct = default) =>
        origin.Add(parser, ct);
}
