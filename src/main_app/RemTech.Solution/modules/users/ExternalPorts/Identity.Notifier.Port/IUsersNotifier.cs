using System.Data;

namespace Identity.Notifier.Port;

public interface IUsersNotifier
{
    Task<UserNotificationResult> Notify(
        UserNotification notification,
        IDbTransaction? transaction = null,
        CancellationToken ct = default
    );
}

public sealed class UserNotificationResult(bool isSuccess, string message)
{
    public bool IsSuccess => isSuccess;
    public string Error => message;
}
