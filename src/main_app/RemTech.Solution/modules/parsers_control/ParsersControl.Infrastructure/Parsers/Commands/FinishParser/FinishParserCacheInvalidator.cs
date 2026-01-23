using ParsersControl.Core.Features.FinishParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.FinishParser;

public sealed class FinishParserCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<FinishParserCommand, SubscribedParser>
{
	public async Task InvalidateCache(
		FinishParserCommand command,
		SubscribedParser result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		await Task.WhenAll(tasks);
	}
}
