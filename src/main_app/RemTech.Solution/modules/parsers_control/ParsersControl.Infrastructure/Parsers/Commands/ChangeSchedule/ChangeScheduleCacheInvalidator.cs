using ParsersControl.Core.Features.ChangeSchedule;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeSchedule;

public sealed class ChangeScheduleCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<ChangeScheduleCommand, SubscribedParser>
{
	public async Task InvalidateCache(
		ChangeScheduleCommand command,
		SubscribedParser result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		await Task.WhenAll(tasks);
	}
}
