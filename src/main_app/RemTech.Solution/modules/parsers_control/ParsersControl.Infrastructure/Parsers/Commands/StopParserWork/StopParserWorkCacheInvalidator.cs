using ParsersControl.Core.Features.StopParserWork;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.StopParserWork;

public sealed class StopParserWorkCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<StopParserWorkCommand, SubscribedParser>
{
	public async Task InvalidateCache(
		StopParserWorkCommand command,
		SubscribedParser result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		await Task.WhenAll(tasks);
	}
}
