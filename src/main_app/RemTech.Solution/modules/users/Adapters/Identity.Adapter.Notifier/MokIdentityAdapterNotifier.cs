using System.Data;
using Identity.Notifier.Port;

namespace Identity.Adapter.Notifier;

public sealed class MokIdentityAdapterNotifier : IUsersNotifier
{
    public async Task<UserNotificationResult> Notify(
        UserNotification notification,
        IDbTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        var result = new UserNotificationResult(true, "Message sent.");
        return await Task.FromResult(result);
    }
}
