using ParsersControl.Core.Features.ChangeLinkActivity;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeLinkActivity;

public sealed class ChangeLinkActivityCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<ChangeLinkActivityCommand, SubscribedParserLink>
{
	public async Task InvalidateCache(
		ChangeLinkActivityCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(command.ParserId, ct)];

		await Task.WhenAll(tasks);
	}
}
