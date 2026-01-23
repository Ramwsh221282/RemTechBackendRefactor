using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.ChangeCredentials;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.ChangeCredentials;

public sealed class ChangeCredentialsCacheInvalidator(
	MailerArrayCacheInvalidator arrayCacheInvalidator,
	MailerRecordCacheInvalidator recordCacheInvalidator
) : ICacheInvalidator<ChangeCredentialsCommand, Mailer>
{
	public Task InvalidateCache(ChangeCredentialsCommand command, Mailer result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayCacheInvalidator.Invalidate(ct), recordCacheInvalidator.Invalidate(result.Id.Value, ct)];

		return Task.WhenAll(tasks);
	}
}
