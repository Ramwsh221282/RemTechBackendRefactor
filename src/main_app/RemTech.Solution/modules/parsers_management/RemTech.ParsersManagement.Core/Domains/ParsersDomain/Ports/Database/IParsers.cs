using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;

public interface IParsers : IDisposable, IAsyncDisposable
{
    Task<Status<IParser>> Find(Name name, CancellationToken ct = default);
    Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default);
    Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    );
    Task<Status> Add(IParser parser, CancellationToken ct = default);
}
