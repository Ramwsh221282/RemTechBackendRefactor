namespace RemTech.Core.Shared.Primitives.Async;

public sealed class AsyncAction(Func<Task> action)
{
    public async Task Execute() => await action();
}