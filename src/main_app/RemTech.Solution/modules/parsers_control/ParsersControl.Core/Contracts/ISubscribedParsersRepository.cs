using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

public interface ISubscribedParsersRepository
{
    public Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default);
    public Task Add(SubscribedParser parser, CancellationToken ct = default);
    public Task Save(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default);
    public Task Save(SubscribedParser parser, CancellationToken ct = default);
    public Task<Result<SubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default);
}
