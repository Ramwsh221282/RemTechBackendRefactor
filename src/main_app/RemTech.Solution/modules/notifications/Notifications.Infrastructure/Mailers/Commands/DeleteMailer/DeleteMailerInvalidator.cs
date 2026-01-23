using Notifications.Core.Mailers.Features.DeleteMailer;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.DeleteMailer;

public sealed class DeleteMailerInvalidator(
	MailerArrayCacheInvalidator arrayCacheInvalidator,
	MailerRecordCacheInvalidator recordCacheInvalidator
) : ICacheInvalidator<DeleteMailerCommand, Guid>
{
	public async Task InvalidateCache(
		DeleteMailerCommand command,
		Guid result,
		CancellationToken ct = new CancellationToken()
	)
	{
		Task[] tasks = [arrayCacheInvalidator.Invalidate(ct), recordCacheInvalidator.Invalidate(result, ct)];

		await Task.WhenAll(tasks);
	}
}
