using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Extensions;

public static class SubscribedParserPersistenceExtensions
{
	extension(SubscribedParser)
	{
		public static async Task<Result<SubscribedParser>> FromRepository(
			ISubscribedParsersRepository repository,
			SubscribedParserQuery query,
			CancellationToken ct = default
		) => await repository.Get(query, ct);
	}

	extension(SubscribedParser parser)
	{
		public async Task SaveChanges(ISubscribedParsersRepository repository, CancellationToken ct = default) =>
			await repository.Save(parser, ct);
	}
}
