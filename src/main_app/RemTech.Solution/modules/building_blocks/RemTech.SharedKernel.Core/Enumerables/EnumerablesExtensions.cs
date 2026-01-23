namespace RemTech.SharedKernel.Core.Enumerables;

public static class EnumerablesExtensions
{
	extension<T>(IEnumerable<T> source)
	{
		public bool HasDuplicates(out T[] duplicates)
		{
			duplicates = source.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
			return duplicates.Length != 0;
		}
	}
}
