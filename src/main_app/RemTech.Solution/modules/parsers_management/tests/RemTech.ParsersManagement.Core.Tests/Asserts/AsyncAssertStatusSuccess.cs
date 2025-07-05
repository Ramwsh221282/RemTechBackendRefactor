using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Tests.Asserts;

public sealed class AsyncAssertStatusSuccess<T>
{
    private readonly Func<Task<Status<T>>> _status;

    public AsyncAssertStatusSuccess(Func<Task<Status<T>>> status) => _status = status;

    public async Task<Status<T>> AsyncAsserted()
    {
        Status<T> status = await _status();
        Assert.True(status.IsSuccess);
        return status;
    }
}
