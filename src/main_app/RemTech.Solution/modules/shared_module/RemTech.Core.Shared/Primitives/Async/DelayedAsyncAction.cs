namespace RemTech.Core.Shared.Primitives.Async;

public sealed class DelayedAsyncAction
{
    private readonly Queue<AsyncAction> _actions = [];

    public void Enqueue(AsyncAction action) => _actions.Enqueue(action);

    public async Task Execute()
    {
        while (_actions.Count > 0)
        {
            AsyncAction action = _actions.Dequeue();
            await action.Execute();
        }
    }
}