using ParsersControl.Core.Features.UpdateParserLink;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.UpdateParserLink;

public sealed class UpdateParserLinkCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<UpdateParserLinkCommand, SubscribedParserLink>
{
	public Task InvalidateCache(
		UpdateParserLinkCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
