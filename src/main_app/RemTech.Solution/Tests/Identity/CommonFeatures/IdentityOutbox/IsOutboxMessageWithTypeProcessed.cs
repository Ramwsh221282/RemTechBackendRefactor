using Identity.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Identity.CommonFeatures.IdentityOutbox;

public sealed class IsOutboxMessageWithTypeProcessed(IServiceProvider sp)
{
    public async Task<bool> Invoke(string type)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IdentityOutboxMessagesStore store = scope.Resolve<IdentityOutboxMessagesStore>();
        IdentityOutboxMessage? message = await store.GetByType(type, ct);
        if (message == null) throw new InvalidOperationException(
            $"Message with type: {type} does not exists in: {nameof(IdentityOutboxMessagesStore)}");
        return message.WasSent;
    }
}