using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Contracts;

public interface ISubscribedParsersRepository
{
    Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default);
    Task Add(SubscribedParser parser, CancellationToken ct = default);
    Task Save(ISubscribedParser parser);
    Task<Result<ISubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default);
}