using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

public interface ISubscribedParsersCollectionRepository
{
	Task<SubscribedParsersCollection> Get(SubscribedParsersCollectionQuery query, CancellationToken ct = default);
	Task<Result<Unit>> SaveChanges(SubscribedParsersCollection collection, CancellationToken ct = default);
}
