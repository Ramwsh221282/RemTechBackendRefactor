using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

public readonly record struct ParsedCount
{
	public int Value { get; private init; }

	public ParsedCount() => Value = 0;

	private ParsedCount(int value) => Value = value;

	public Result<ParsedCount> Add(int amount)
	{
		if (amount < 0)
			return Error.Validation("Количество добавляемых данных парсером не может быть отрицательным.");
		return new ParsedCount(Value + amount);
	}

    public static Result<ParsedCount> Create(int value) => value < 0
            ? Error.Validation("Количество обработанных данных парсером не может быть отрицательным.")
            : new ParsedCount(value);

    public static ParsedCount New() => Create(0).Value;
}
