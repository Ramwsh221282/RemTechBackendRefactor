using System.Diagnostics;

namespace RemTech.Core.Shared.Primitives;

public sealed class SwitchReturn<Tin, TOut>
{
    private readonly List<(bool, Func<TOut> whatReturn)> _list;
    private readonly Tin _value;
    private Func<Tin, TOut>? _defaultReturn;

    public SwitchReturn(Tin value, Func<Tin, TOut> defaultReturn)
        : this(value)
    {
        _defaultReturn = defaultReturn;
    }

    public SwitchReturn(Tin value)
    {
        _value = value;
        _list = [];
    }

    public SwitchReturn<Tin, TOut> With(Func<Tin, bool> conditionFn, Func<Tin, TOut> whatReturn)
    {
        Func<TOut> outFn = () => whatReturn(_value);
        _list.Add((conditionFn(_value), outFn));
        return this;
    }

    public SwitchReturn<Tin, TOut> Default(Func<Tin, TOut> defaultReturn)
    {
        _defaultReturn = defaultReturn;
        return this;
    }

    public TOut Return()
    {
        foreach (var entry in _list)
        {
            if (entry.Item1)
                return entry.whatReturn();
        }

        if (_defaultReturn != null)
            return _defaultReturn(_value);

        throw new UnreachableException("Switch return unreachable.");
    }
}
