using System.ComponentModel;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace WebHostApplication.Modules.Telemetry;

[TypeConverter(typeof(SortDictionaryQueryParameterConverter))]
public sealed class SortDictionary : Dictionary<string, string>
{
	private const char SORT_RELATION_SEPARATOR = ':';
	private const char ARGUMENTS_SEPARATOR = ',';

	public static SortDictionary FromString(string? input)
	{
		SortDictionary dictionary = [];
		if (string.IsNullOrWhiteSpace(input))
		{
			return dictionary;
		}

		string[] parts = SplitArguments(input);
		AddSortParts(dictionary, parts);
		return dictionary;
	}

	private static void AddSortParts(SortDictionary dictionary, string[] parts)
	{
		foreach (string part in parts)
		{
			Optional<KeyValuePair<string, string>> sortClause = ParseArgument(part);
			if (!sortClause.HasValue)
			{
				continue;
			}

			if (dictionary.ContainsKey(sortClause.Value.Key))
			{
				continue;
			}

			dictionary.Add(sortClause.Value.Key, sortClause.Value.Value);
		}
	}

	private static Optional<KeyValuePair<string, string>> ParseArgument(string argument)
	{
		string[] field_sortMode = argument.Split(SORT_RELATION_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
		if (field_sortMode.Length != 2)
		{
			return Optional.None<KeyValuePair<string, string>>();
		}

		string field = field_sortMode[0];
		string sortMode = field_sortMode[1];
		if (sortMode == "ASC" || sortMode == "DESC" || sortMode == "NONE")
		{
			return new KeyValuePair<string, string>(field, sortMode);
		}

		return Optional.None<KeyValuePair<string, string>>();
	}

	private static string[] SplitArguments(string input)
	{
		return input.Split(ARGUMENTS_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
	}
}
