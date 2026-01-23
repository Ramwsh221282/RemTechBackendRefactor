using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyDisableManyParsing;

public sealed class PermantlyDisableManyParsingCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>>
{
	public async Task InvalidateCache(
		PermantlyDisableManyParsingCommand command,
		IEnumerable<SubscribedParser> result,
		CancellationToken ct = default
	)
	{
		IEnumerable<Task> recordInvalidationTasks =
		[
			arrayInvalidator.Invalidate(ct),
			.. result.Select(p => recordInvalidator.Invalidate(p, ct)),
		];

		await Task.WhenAll(recordInvalidationTasks);
	}
}
