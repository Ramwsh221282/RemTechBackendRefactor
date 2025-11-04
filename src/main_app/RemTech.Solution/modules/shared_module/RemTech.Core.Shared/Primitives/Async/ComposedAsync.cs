using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives.Async;

public sealed class ComposedAsync<Y, X> : IAsync<Y, X> where Y :
    class
    where X : class

{
    private readonly Queue<IAsync<Y, X>> _functions = [];
    public void Add(IAsync<Y, X> function) => _functions.Enqueue(function);

    public async Task<Status<X>> ExecuteAsync(Y component)
    {
        Status<X> lastResult = default!;
        while (_functions.Count > 0)
        {
            lastResult = await _functions.Dequeue().ExecuteAsync(component);
            if (lastResult.IsFailure)
                return lastResult;
        }

        return lastResult;
    }
}