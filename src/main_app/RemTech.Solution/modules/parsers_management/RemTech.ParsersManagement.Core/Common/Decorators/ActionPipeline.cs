namespace RemTech.ParsersManagement.Core.Common.Decorators;

public sealed class ActionPipeline<T>
{
    private readonly T _value;
    private readonly Queue<(Func<T, bool> predicate, Action<T> action)> _actions;

    public ActionPipeline(T value)
    {
        _value = value;
        _actions = [];
    }

    public ActionPipeline<T> With(Func<T, bool> predicate, Action<T> action)
    {
        _actions.Enqueue((predicate, action));
        return this;
    }

    public T Process()
    {
        while (_actions.Count > 0)
        {
            (Func<T, bool> predicate, Action<T> action) = _actions.Dequeue();
            bool result = predicate(_value);
            if (result)
                action(_value);
        }
        return _value;
    }
}
