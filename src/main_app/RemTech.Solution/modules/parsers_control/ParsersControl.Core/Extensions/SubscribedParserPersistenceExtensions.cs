using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Extensions;

/// <summary>
/// Расширения для сохранения подписанных парсеров.
/// </summary>
public static class SubscribedParserPersistenceExtensions
{
	extension(SubscribedParser)
	{
		public static Task<Result<SubscribedParser>> FromRepository(
			ISubscribedParsersRepository repository,
			SubscribedParserQuery query,
			CancellationToken ct = default
		) => repository.Get(query, ct);
	}

	extension(SubscribedParser parser)
	{
		public Task SaveChanges(ISubscribedParsersRepository repository, CancellationToken ct = default) =>
			repository.Save(parser, ct);
	}
}
