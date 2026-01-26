using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.PrimitivesModule;

public static class EnumerablesImplementation
{
	extension<T>(IEnumerable<T> enumerable)
	{
		public U[] Map<U>(Func<T, U> func) => enumerable.ToArray().Map(func);
	}

	extension<T>(T[] array)
	{
		public U[] Map<U>(Func<T, U> func)
		{
			const int startIndex = 0;
			return array.MapRecursive(new U[array.Length], startIndex, func);
		}

		public void ForEach(Action<T> action)
		{
			foreach (T item in array)
				action(item);
		}

		public Optional<T> TryFind(Func<T, bool> func)
		{
			T? element = array.FirstOrDefault(func);
			return element == null ? Optional.None<T>() : Optional.Some(element);
		}

		public T[] With(T item) => [item, .. array];

		public T[] Without(Func<T, bool> removeCriteria) => [.. array.Where(removeCriteria).ToArray()];

		private U[] MapRecursive<U>(U[] result, int index, Func<T, U> func)
		{
			if (index >= array.Length)
				return result;
			result[index] = func(array[index]);
			int nextIndex = index + 1;
			return array.MapRecursive(result, nextIndex, func);
		}
	}
}
