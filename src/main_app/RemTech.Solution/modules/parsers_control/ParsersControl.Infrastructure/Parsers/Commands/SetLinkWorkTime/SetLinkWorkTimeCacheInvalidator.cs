using ParsersControl.Core.Features.SetLinkWorkTime;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetLinkWorkTime;

public sealed class SetLinkWorkTimeCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<SetLinkWorkingTimeCommand, SubscribedParserLink>
{
	public Task InvalidateCache(
		SetLinkWorkingTimeCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(command.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
