using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.StartParserWork;

public sealed class StartParserCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<StartParserCommand, SubscribedParser>
{
	public async Task InvalidateCache(
		StartParserCommand command,
		SubscribedParser result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		await Task.WhenAll(tasks);
	}
}
