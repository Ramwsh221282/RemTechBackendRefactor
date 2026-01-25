using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SpareAddress
{
	private SpareAddress(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<SpareAddress> Create(string value) =>
		string.IsNullOrWhiteSpace(value) ? Error.Validation("Адрес не может быть пустым") : new SpareAddress(value);
}
