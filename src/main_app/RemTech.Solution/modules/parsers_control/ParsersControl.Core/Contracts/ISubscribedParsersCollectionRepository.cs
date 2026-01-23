using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

public interface ISubscribedParsersCollectionRepository
{
	public Task<SubscribedParsersCollection> Get(
		SubscribedParsersCollectionQuery query,
		CancellationToken ct = default
	);
	public Task<Result<Unit>> SaveChanges(SubscribedParsersCollection collection, CancellationToken ct = default);
}
