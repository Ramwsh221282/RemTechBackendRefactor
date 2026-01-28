namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

/// <summary>
/// Расширения для работы с уникальными последовательностями.
/// </summary>
public static class UniqueSequenceExtensions
{
	extension<T>(IEnumerable<T> items)
	{
		public Result<UniqueSequence<T>> TryBecomeUnique() => UniqueSequence<T>.Create(items);

		public Result<UniqueSequence<T>> TryBecomeUnique(string onError) => UniqueSequence<T>.Create(items, onError);

		public Result<UniqueSequence<T>> TryBecomeUnique<TSource>(Func<T, TSource> selector) =>
			UniqueSequence<T>.Create(items, selector);

		public Result<UniqueSequence<T>> TryBecomeUnique<TSource>(Func<T, TSource> selector, string onError) =>
			UniqueSequence<T>.Create(items, selector, onError);
	}
}
