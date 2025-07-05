using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Asserts;

public class AsyncAssertStatusFailure<T>
{
    private readonly Func<Task<Status<T>>> _status;

    public AsyncAssertStatusFailure(Func<Task<Status<T>>> status) => _status = status;

    public async Task<Status<T>> AsyncAsserted()
    {
        Status<T> status = await _status();
        Assert.True(status.IsFailure);
        return status;
    }
}
