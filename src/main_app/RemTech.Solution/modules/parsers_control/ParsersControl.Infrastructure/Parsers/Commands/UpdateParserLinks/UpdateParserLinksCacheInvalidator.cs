using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.UpdateParserLinks;

public sealed class UpdateParserLinksCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>>
{
	public async Task InvalidateCache(
		UpdateParserLinksCommand command,
		IEnumerable<SubscribedParserLink> result,
		CancellationToken ct = default
	)
	{
		Task[] tasks =
		[
			arrayInvalidator.Invalidate(ct),
			Task.WhenAll(result.Select(link => recordInvalidator.Invalidate(link.ParserId, ct))),
		];

		await Task.WhenAll(tasks);
	}
}
