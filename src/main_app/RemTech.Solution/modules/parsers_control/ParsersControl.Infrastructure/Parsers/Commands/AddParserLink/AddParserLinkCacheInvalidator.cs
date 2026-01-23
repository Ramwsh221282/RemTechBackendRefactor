using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.AddParserLink;

public sealed class AddParserLinkCacheInvalidator(
	CachedParserArrayInvalidator invalidator,
	ParserCacheRecordInvalidator cacheRecordInvalidator
) : ICacheInvalidator<AddParserLinkCommand, IEnumerable<SubscribedParserLink>>
{
	public async Task InvalidateCache(
		AddParserLinkCommand command,
		IEnumerable<SubscribedParserLink> result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [invalidator.Invalidate(ct), cacheRecordInvalidator.Invalidate(command.ParserId, ct)];

		await Task.WhenAll(tasks);
	}
}
