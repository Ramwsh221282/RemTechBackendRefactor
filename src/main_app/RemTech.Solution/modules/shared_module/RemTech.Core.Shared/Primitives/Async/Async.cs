using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives.Async;

public sealed class Async<Y, X>(Func<Y, Task<Status<X>>> function) : IAsync<Y, X> where X : class where Y : class
{
    public async Task<Status<X>> ExecuteAsync(Y component)
    {
        Status<X> result = await function(component);
        return result;
    }
}