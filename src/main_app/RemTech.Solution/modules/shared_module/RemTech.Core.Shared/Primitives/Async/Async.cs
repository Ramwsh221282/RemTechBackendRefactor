using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives.Async;

public sealed class Async<Y, X>(Func<Y, Task<X>> function) : IAsync<Y, X> where X : class where Y : class
{
    public async Task<Status<X>> ExecuteAsync(Y component)
    {
        Task<X> task = function(component);
        return await task;
    }
}