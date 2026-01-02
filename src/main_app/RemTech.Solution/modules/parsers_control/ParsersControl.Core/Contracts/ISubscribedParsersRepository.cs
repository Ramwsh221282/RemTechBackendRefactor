using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

public interface ISubscribedParsersRepository
{
    Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default);
    Task Add(SubscribedParser parser, CancellationToken ct = default);
    Task Save(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default);
    Task Save(SubscribedParser parser, CancellationToken ct = default);
    Task<Result<SubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default);
}