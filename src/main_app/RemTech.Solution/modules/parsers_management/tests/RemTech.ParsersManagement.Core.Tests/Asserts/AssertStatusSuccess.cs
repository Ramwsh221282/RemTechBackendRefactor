using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Asserts;

public class AssertStatusSuccess<T>
{
    private readonly Func<Status<T>> _statusFn;

    public AssertStatusSuccess(Func<Status<T>> statusFn) => _statusFn = statusFn;

    public Status<T> Asserted()
    {
        Status<T> status = _statusFn();
        Assert.True(status.IsSuccess);
        return status;
    }
}
