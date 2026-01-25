using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareType
{
	private SpareType(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<SpareType> Create(string value) =>
		string.IsNullOrWhiteSpace(value) ? Error.Validation("Тип запчасти не может быть пустым") : new SpareType(value);
}
