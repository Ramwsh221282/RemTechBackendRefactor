using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.AddMailer;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.AddMailer;

public sealed class AddMailerCacheInvalidator(
    MailerArrayCacheInvalidator arrayInvalidator,
    MailerRecordCacheInvalidator recordInvalidator
) : ICacheInvalidator<AddMailerCommand, Mailer>
{
    public async Task InvalidateCache(
        AddMailerCommand command,
        Mailer result,
        CancellationToken ct = default
    )
    {
        Task[] tasks =
        [
            arrayInvalidator.Invalidate(ct),
            recordInvalidator.Invalidate(result.Id.Value, ct),
        ];

        await Task.WhenAll(tasks);
    }
}
