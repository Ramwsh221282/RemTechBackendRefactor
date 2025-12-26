using Identity.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;

namespace Tests.Identity.CommonFeatures.IdentityOutbox;

public sealed class HasIdentityOutboxWithType(IServiceProvider sp)
{
    public async Task<bool> Invoke(string type)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IdentityOutboxMessagesStore store = scope.Resolve<IdentityOutboxMessagesStore>();
        IdentityOutboxMessage? message = await store.GetByType(type, ct);
        return message != null;
    }
}