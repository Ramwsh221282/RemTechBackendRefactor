using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Asserts;

public class AssertStatusFailure<T>
{
    private readonly Func<Status<T>> _statusFn;

    public AssertStatusFailure(Func<Status<T>> statusFn) => _statusFn = statusFn;

    public Status<T> Asserted()
    {
        Status<T> status = _statusFn();
        Assert.True(status.IsFailure);
        return status;
    }
}
