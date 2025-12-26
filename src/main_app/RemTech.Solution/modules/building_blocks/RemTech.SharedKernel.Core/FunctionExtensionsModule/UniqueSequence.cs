namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public sealed class UniqueSequence<T>
{
    private readonly HashSet<T> _items;
    
    internal UniqueSequence(HashSet<T> items)
    {
        _items = items;   
    }

    public IEnumerable<U> Map<U>(Func<T, U> selector)
    {
        return _items.Select(selector);
    }
    
    public U[] MapToArray<U>(Func<T, U> factory)
    {
        int count = _items.Count;
        U[] array = new U[count];
        int current = 0;
        foreach (T item in _items)
        {
            array[current] = factory(item);
            current++;
        }
        return array;
    }
    
    public static Result<UniqueSequence<T>> Create(IEnumerable<T> items)
    {
        HashSet<T> set = [];
        
        foreach (T item in items)
        {
            if (!set.Add(item))
                return Error.Validation("Коллекция не уникальна.");
        }
        
        return new UniqueSequence<T>(set);
    }

    public static Result<UniqueSequence<T>> Create(IEnumerable<T> items, string onError)
    {
        Result<UniqueSequence<T>> sequence = Create(items);
        return sequence.IsFailure ? Error.Validation(onError) : sequence;
    }
    
    public static Result<UniqueSequence<T>> Create<TSource>(
        IEnumerable<T> items,
        Func<T, TSource> keySelector,
        IEqualityComparer<TSource>? comparer = null)
    {
        comparer ??= EqualityComparer<TSource>.Default;
        HashSet<TSource> seenKeys = new(comparer);
        HashSet<T> result = [];

        foreach (T item in items)
        {
            var key = keySelector(item);
            if (!seenKeys.Add(key))
                return Error.Validation("Коллекция не уникальна.");
        
            result.Add(item);
        }

        return new UniqueSequence<T>(result);
    }
    
    public static Result<UniqueSequence<T>> Create<TSource>(
        IEnumerable<T> items,
        Func<T, TSource> keySelector,
        string onError,
        IEqualityComparer<TSource>? comparer = null)
    {
        Result<UniqueSequence<T>> sequence = Create(items, keySelector, comparer);
        return sequence.IsFailure ? Error.Validation(onError) : sequence;
    }
}

public static class UniqueSequenceExtensions
{
    extension<T>(IEnumerable<T> items)
    {
        public Result<UniqueSequence<T>> TryBecomeUnique()
        {
            return UniqueSequence<T>.Create(items);
        }
        
        public Result<UniqueSequence<T>> TryBecomeUnique(string onError)
        {
            return UniqueSequence<T>.Create(items, onError);
        }
        
        public Result<UniqueSequence<T>> TryBecomeUnique<TSource>(Func<T, TSource> selector)
        {
            return UniqueSequence<T>.Create(items, selector);
        }
        
        public Result<UniqueSequence<T>> TryBecomeUnique<TSource>(Func<T, TSource> selector, string onError)
        {
            return UniqueSequence<T>.Create(items, selector, onError);
        }
    }
}