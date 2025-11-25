using Identity.Contracts.Accounts;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class FakeAccountMessagePublisher : IAccountMessagePublisher
{
    public async Task Publish<TMessage>(TMessage message, CancellationToken ct = default) where TMessage : AccountMessage
    {
        await Task.Yield();
    }
}